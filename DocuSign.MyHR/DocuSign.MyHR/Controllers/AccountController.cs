using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthenticationService = DocuSign.MyHR.Security.IAuthenticationService;

namespace DocuSign.MyHR.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> Login(string authType = "CodeGrant", string returnUrl = "/employee")
        {
            if (authType == "CodeGrant")
            {
                return Challenge(new AuthenticationProperties {RedirectUri = returnUrl});
            }

            if (authType == "JWT")
            {
                var authResult = _authenticationService.AuthenticateFromJwt();
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    authResult.Item1,
                    authResult.Item2);
                return LocalRedirect(returnUrl);
            }
              
            return BadRequest("Unknown authentication type");
        }

        [Authorize]
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> Logout()
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext);
            return LocalRedirect("/");
        }
         
        [HttpGet]
        [Route("/api/isauthenticated")]
        public IActionResult IsAuthenticated()
        { 
            return Ok(User.Identity.IsAuthenticated);
        }
    }
}