using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;

namespace DocuSign.MyHR.Services.TemplateHandlers
{
    public interface ITemplateHandler
    {
        EnvelopeTemplate BuildTemplate(string rootDir);
        EnvelopeDefinition BuildEnvelope(UserDetails currentUser, UserDetails additionalUser);
        string TemplateName { get; }
    }
}