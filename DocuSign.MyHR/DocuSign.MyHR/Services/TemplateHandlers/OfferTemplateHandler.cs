using System;
using System.Collections.Generic;
using System.IO;
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Services.TemplateHandlers
{
    public class OfferTemplateHandler : ITemplateHandler
    {
        private string _signerClientId = "1000";
        private string _templatePath = "/Templates/EmploymentOfferLetter.json";
        public string TemplateName => "Employment Offer Letter Sample";
        public EnvelopeTemplate BuildTemplate(string rootDir)
        {
            using var reader = new StreamReader(rootDir + _templatePath);
            return JsonConvert.DeserializeObject<EnvelopeTemplate>(reader.ReadToEnd());
        }

        public EnvelopeDefinition BuildEnvelope(UserDetails currentUser, UserDetails additionalUser)
        {
            if (currentUser == null)
            {
                throw new ArgumentNullException(nameof(currentUser));
            }

            if (additionalUser == null)
            {
                throw new ArgumentNullException(nameof(additionalUser));
            } 

            var roleHr = new TemplateRole
            {
                Email = currentUser.Email,
                Name = currentUser.Name,
                ClientUserId = _signerClientId,
                RoleName = "HR Rep"
            };
            var roleNewHire = new TemplateRole
            {
                Email = additionalUser.Email,
                Name = additionalUser.Name, 
                RoleName = "New Hire"
            };

            var env = new EnvelopeDefinition
            {
                TemplateRoles = new List<TemplateRole> {roleHr, roleNewHire}, Status = "sent"
            };
            return env;
        }
    }
}
