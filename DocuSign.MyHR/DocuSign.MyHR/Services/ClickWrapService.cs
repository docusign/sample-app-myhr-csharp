using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using DocumentFormat.OpenXml.Packaging;
using System.Net.Http;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using DocuSign.MyHR.Extensions;

namespace DocuSign.MyHR.Services
{
    public class ClickWrapService : IClickWrapService
    {
        private readonly IDocuSignApiProvider _docuSignApiProvider;
        private readonly IConfiguration _configuration;
        private string _templatePath = "/Templates/Time Tracking Confirmation.dotx";
        private string _tempPath = "/Templates/Time Tracking Confirmation{0}_tmp.docx";

        public ClickWrapService(IDocuSignApiProvider docuSignApiProvider, IConfiguration configuration)
        {
            _docuSignApiProvider = docuSignApiProvider;
            _configuration = configuration;
        }

        public HttpResponseMessage CreateTimeTrackClickWrap(string accountId, string userId, int[] workingLog)
        {
            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (workingLog == null)
            {
                throw new ArgumentNullException(nameof(workingLog));
            }

            if (workingLog.Length < 5)
            {
                throw new InvalidOperationException("Work log must be provided for all working days");
            }

            var createResponse = CreateClickWrap(accountId, userId, workingLog);
            if (createResponse.StatusCode != HttpStatusCode.Created)
            {
                throw new InvalidOperationException($"ClickWrap was not created. " +
                                                    $"Returned status code {createResponse.StatusCode}, reason {createResponse.ReasonPhrase}");
            }

            var response = JsonConvert.DeserializeObject<dynamic>(createResponse.Content.ReadAsStringAsync().Result);
            return ActivateClickWrap(accountId, response.clickwrapId.ToString());
        }

        private HttpResponseMessage CreateClickWrap(string accountId, string userId, int[] workingLog)
        {
            var rootDir = _configuration.GetValue<string>(WebHostDefaults.ContentRootKey);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"clickapi/v1/accounts/{accountId}/clickwraps");

            var requestBody = new
            {
                displaySettings = new
                {
                    consentButtonText = "I Confirm",
                    hasDeclineButton = true,
                    displayName = "Time Tracking Confirmation",
                    downloadable = true,
                    format = "modal",
                    hasAccep = true,
                    mustRead = true,
                    requireAccept = true,
                    documentDisplay = "document",
                    Size = "small"
                },
                documents = new[]
                {
                    new
                    {
                        documentBase64 = GetDocumentBase64(workingLog, rootDir, userId),
                        documentName = "Time Tracking Confirmation",
                        fileExtension = "docx",
                        order = 0
                    }
                },
                name = "Time Tracking Confirmation",
                status = "active"
            };
            request.Content = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json");

            return _docuSignApiProvider.DocuSignHttpClient.SendAsync(request).Result;
        }

        private HttpResponseMessage ActivateClickWrap(string accountId, string clickwrapId)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"clickapi/v1/accounts/{accountId}/clickwraps/{clickwrapId}/versions/1");
            var requestBody = new
            {
                status = "active"
            };
            request.Content = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json");

            return _docuSignApiProvider.DocuSignHttpClient.SendAsync(request).Result;
        }

        private string GetDocumentBase64(int[] workingLog, string rootDir, string userId)
        {
            var tempDocPath = rootDir + string.Format(_tempPath, userId);
            using (var doc = WordprocessingDocument.CreateFromTemplate(rootDir + _templatePath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                foreach (var text in body.Descendants<Text>())
                {
                    if (text.Text.Contains("hrs"))
                    {
                        text.Text = text.Text.Replace("hrs", workingLog.Sum().ToString());
                    }
                }

                doc.Clone(tempDocPath).Dispose();
            }

            using var memoryStream = new MemoryStream(File.ReadAllBytes(tempDocPath));
            var docBase64 = Convert.ToBase64String(memoryStream.ReadAsBytes());
            File.Delete(tempDocPath);
            return docBase64;
        }
    }
}
