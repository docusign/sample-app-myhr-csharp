using System;
using System.Collections.Generic;
using System.IO;
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Services.TemplateHandlers
{
    public class I9TemplateHandler : ITemplateHandler
    {
        private string _templatePath = "/Templates/I-9_2020.json";
        public string TemplateName => "I-9 2020 Sample";

        public EnvelopeTemplate BuildTemplate(string rootDir)
        {
            using var reader =  new StreamReader(rootDir + _templatePath);
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
                RoleName = "HR"
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
