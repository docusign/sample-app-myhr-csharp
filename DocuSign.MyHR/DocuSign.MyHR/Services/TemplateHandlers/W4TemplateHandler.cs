using System;
using System.Collections.Generic;
using System.IO; 
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Services.TemplateHandlers
{
    public class W4TemplateHandler : ITemplateHandler
    {
        private string _signerClientId = "1000";
        private string _templatePath = "/Templates/W-4_2020.json";
        public string TemplateName => "W-4 2020 Sample";

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
           
            var role = new TemplateRole
            {
                Email = currentUser.Email,
                Name = currentUser.Name,
                RoleName = "New Hire",
                ClientUserId = _signerClientId
            };
           
            var env = new EnvelopeDefinition {TemplateRoles = new List<TemplateRole> {role}, Status = "sent"};

            return env;
        } 
    }
}
