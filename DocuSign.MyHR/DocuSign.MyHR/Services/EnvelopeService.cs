using System;
using System.Collections.Generic;
using System.Linq;
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Exceptions;
using DocuSign.MyHR.Services.TemplateHandlers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DocuSign.MyHR.Services
{
    public class EnvelopeService : IEnvelopeService
    {
        private string _signerClientId = "1000";
        private readonly IDocuSignApiProvider _docuSignApiProvider;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public EnvelopeService(IDocuSignApiProvider docuSignApiProvider, IUserService userService, IConfiguration configuration)
        {
            _docuSignApiProvider = docuSignApiProvider;
            _userService = userService;
            _configuration = configuration;
        }

        public CreateEnvelopeResponse CreateEnvelope(
            DocumentType type,
            string accountId,
            string userId,
            LoginType loginType,
            UserDetails additionalUser,
            string redirectUrl,
            string pingAction)
        {
            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            string rootDir = _configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
            UserDetails userDetails = _userService.GetUserDetails(accountId, userId, loginType);

            var templateHandler = GetTemplateHandler(type);
            EnvelopeTemplate envelopeTemplate = templateHandler.BuildTemplate(rootDir);
            EnvelopeDefinition envelope = templateHandler.BuildEnvelope(userDetails, additionalUser);

            if (type == DocumentType.I9 && loginType == LoginType.JWT)
            {
                EnableIDV(accountId, envelopeTemplate, "New Hire");
            }

            var listTemplates = _docuSignApiProvider.TemplatesApi.ListTemplates(accountId);
            EnvelopeTemplate template = listTemplates?.EnvelopeTemplates?.FirstOrDefault(x => x.Name == templateHandler.TemplateName);

            if (template != null)
            {
                envelope.TemplateId = template.TemplateId;
            }
            else
            {
                TemplateSummary templateSummary = _docuSignApiProvider.TemplatesApi.CreateTemplate(accountId, envelopeTemplate);
                envelope.TemplateId = templateSummary.TemplateId;
            }
            EnvelopeSummary envelopeSummary = _docuSignApiProvider.EnvelopApi.CreateEnvelope(accountId, envelope);

            if (type != DocumentType.I9)
            {
                ViewUrl recipientView = _docuSignApiProvider.EnvelopApi.CreateRecipientView(
                    accountId,
                    envelopeSummary.EnvelopeId,
                    BuildRecipientViewRequest(
                        userDetails.Email,
                        userDetails.Name,
                        redirectUrl,
                        pingAction)
                    );
                return new CreateEnvelopeResponse(recipientView.Url, envelopeSummary.EnvelopeId);
            }

            return new CreateEnvelopeResponse(string.Empty, envelopeSummary.EnvelopeId);
        }

        public Dictionary<string, string> GetEnvelopData(string accountId, string envelopeId)
        {
            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }
            if (envelopeId == null)
            {
                throw new ArgumentNullException(nameof(envelopeId));
            }

            EnvelopeFormData results = _docuSignApiProvider.EnvelopApi.GetFormData(accountId, envelopeId);
            return results.FormData.ToDictionary(x => x.Name, x => x.Value);
        }

        private void EnableIDV(string accountId, EnvelopeTemplate envelopeTemplate, string roleName)
        {
            AccountIdentityVerificationResponse idvResponse = _docuSignApiProvider.AccountsApi.GetAccountIdentityVerification(accountId);
            var workflowId = idvResponse.IdentityVerification.FirstOrDefault()?.WorkflowId;
            if (workflowId == null)
            {
                throw new IDVException("IdentityVerification workflow is not found. Check that IDV is enabled in Docusign account.");
            }
            foreach (Signer recipient in envelopeTemplate.Recipients.Signers.Where(x => x.RoleName == roleName))
            {
                recipient.IdentityVerification = new RecipientIdentityVerification
                {
                    WorkflowId = workflowId
                };
            }
        }

        private RecipientViewRequest BuildRecipientViewRequest(string signerEmail, string signerName, string returnUrl, string pingUrl)
        {
            RecipientViewRequest viewRequest = new RecipientViewRequest
            {
                ReturnUrl = returnUrl,
                AuthenticationMethod = "none",
                Email = signerEmail,
                UserName = signerName,
                ClientUserId = _signerClientId
            };

            if (pingUrl != null)
            {
                viewRequest.PingFrequency = "600";
                viewRequest.PingUrl = pingUrl;
            }

            return viewRequest;
        }

        private ITemplateHandler GetTemplateHandler(DocumentType type)
        {
            ITemplateHandler templateHandler;
            switch (type)
            {
                case DocumentType.I9:
                    templateHandler = new I9TemplateHandler();
                    break;
                case DocumentType.W4:
                    templateHandler = new W4TemplateHandler();
                    break;
                case DocumentType.Offer:
                    templateHandler = new OfferTemplateHandler();
                    break;
                case DocumentType.DirectDeposit:
                    templateHandler = new DirectDepositTemplateHandler();
                    break;
                case DocumentType.TuitionRbt:
                    templateHandler = new TuitionReimbursementTemplateHandler();
                    break;
                default:
                    throw new InvalidOperationException("Document type is not set");
            }

            return templateHandler;
        }
    }
}
