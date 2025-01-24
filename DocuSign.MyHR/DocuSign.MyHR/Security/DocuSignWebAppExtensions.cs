using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using DocuSign.eSign.Client;
using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace DocuSign.MyHR.Security
{
    [ExcludeFromCodeCoverage]
    public static class DocuSignWebAppExtensions
    {
        public static void ConfigureDocuSign(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseAuthentication();
            applicationBuilder.UseSession();
        }

        public static void ConfigureDocuSignSSO(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddSession();
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication(options =>
            {
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "DocuSign";
            })
            .AddOAuth("DocuSign", options =>
            {
                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

                options.ClientId = configuration["DocuSign:IntegrationKey"];
                options.ClientSecret = configuration["DocuSign:SecretKey"];
                options.CallbackPath = new PathString("/ds/callback");
                options.AuthorizationEndpoint = configuration["DocuSign:AuthorizationEndpoint"];
                options.TokenEndpoint = configuration["DocuSign:TokenEndpoint"];
                options.UserInformationEndpoint = configuration["DocuSign:UserInformationEndpoint"];

                options.Scope.Add("signature");
                options.Scope.Add("click.manage");
                options.SaveTokens = true;

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapJsonKey("accounts", "accounts");
                options.ClaimActions.MapJsonKey("authType", "authType");
                options.ClaimActions.MapCustomJson("account_id", obj => ExtractDefaultAccountValue(obj, "account_id"));

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Add("X-Content-Type-Options", "nosniff");
                        request.Headers.Add("X-Frame-Options", "deny");
                        request.Headers.Add("Cache-Control", "no-cache");
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var user = JObject.Parse(await response.Content.ReadAsStringAsync());
                        user.Add("authType", LoginType.CodeGrant.ToString());

                        using JsonDocument payload = JsonDocument.Parse(user.ToString());

                        context.RunClaimActions(payload.RootElement);
                    },
                    OnRemoteFailure = context =>
                    {
                        context.HandleResponse();
                        context.Response.WriteAsync(new ErrorDetails
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = $"Internal Server Error: {context.Failure.Message}"
                        }.ToString());
                        return Task.FromResult(0);
                    },
                    OnRedirectToAuthorizationEndpoint = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.Headers["Location"] = context.RedirectUri;
                            context.Response.StatusCode = 401;
                        }
                        else
                        {
                            context.Response.Redirect(context.RedirectUri);
                        }

                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
            {
                config.Cookie.Name = "UserLoginCookie";
                config.Cookie.HttpOnly = true;
                config.Cookie.SameSite = SameSiteMode.Lax;
                config.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                config.LoginPath = "/Account/Login";
                config.SlidingExpiration = true;
                config.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                config.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                   {
                       // Return 401 HttpCode for api calls instead of redirecting to login page
                       if (context.Request.Path.StartsWithSegments("/api"))
                       {
                           context.Response.Headers["Location"] = context.RedirectUri;
                           context.Response.StatusCode = 401;
                       }
                       else
                       {
                           context.Response.Redirect(context.RedirectUri);
                       }

                       return Task.CompletedTask;
                   },
                    // Check access token expiration and refresh if expired
                    OnValidatePrincipal = context =>
                    {
                        if (context.Properties.Items.ContainsKey(".Token.expires_at"))
                        {
                            var expire = DateTime.Parse(context.Properties.Items[".Token.expires_at"]);
                            if (expire < DateTime.Now)
                            {
                                var authProperties = context.Properties;
                                var options = context.HttpContext.RequestServices
                                    .GetRequiredService<IOptionsMonitor<OAuthOptions>>()
                                    .Get("DocuSign");

                                var requestParameters = new Dictionary<string, string>
                                {
                                    {"client_id", configuration["DocuSign:IntegrationKey"]},
                                    {"client_secret", configuration["DocuSign:SecretKey"]},
                                    {"grant_type", "refresh_token"},
                                    {"refresh_token", authProperties.GetTokenValue("refresh_token")}
                                };

                                // Request new access token with refresh token
                                var refreshResponse = options.Backchannel.PostAsync(
                                    options.TokenEndpoint,
                                    new FormUrlEncodedContent(requestParameters),
                                    context.HttpContext.RequestAborted).Result;
                                refreshResponse.EnsureSuccessStatusCode();

                                var payload = JObject.Parse(refreshResponse.Content.ReadAsStringAsync().Result);

                                // Persist the new access token and refresh token
                                authProperties.UpdateTokenValue("access_token", payload.Value<string>("access_token"));
                                authProperties.UpdateTokenValue("refresh_token", payload.Value<string>("refresh_token"));

                                if (int.TryParse(
                                    payload.Value<string>("expires_in"),
                                    NumberStyles.Integer,
                                    CultureInfo.InvariantCulture, out var seconds))
                                {
                                    var expiresAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(seconds);
                                    authProperties.UpdateTokenValue(
                                        "expires_at",
                                        expiresAt.ToString("o", CultureInfo.InvariantCulture));
                                }
                                context.ShouldRenew = true;
                            }
                        }
                        return Task.FromResult(0);
                    }
                };
            });
            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    "DocuSign",
                    CookieAuthenticationDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

        }

        public static void ConfigureDocuSignExceptionHandling(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<Startup>();
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        if (contextFeature.Error is ApiException apiError)
                        {
                            logger.LogError($"Error occured during Docusign api call: {contextFeature.Error}");

                            if (apiError.ErrorCode == (int)HttpStatusCode.Unauthorized)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            }
                        }
                        else
                        {
                            logger.LogError($"Error occured: {contextFeature.Error}");
                        }
                        await context.Response.WriteAsync(new ErrorDetails
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error."
                        }.ToString());
                    }
                });
            });
        }

        private static string ExtractDefaultAccountValue(JsonElement obj, string key)
        {
            if (!obj.TryGetProperty("accounts", out var accounts))
            {
                return null;
            }

            string keyValue = null;

            foreach (var account in accounts.EnumerateArray())
            {
                if (account.TryGetProperty("is_default", out var defaultAccount) && defaultAccount.GetBoolean())
                {
                    if (account.TryGetProperty(key, out var value))
                    {
                        keyValue = value.GetString();
                    }
                }
            }

            return keyValue;
        }
    }
}
