using DocuSign.MyHR.Domain;

namespace DocuSign.MyHR.Models
{
    public class RequestEnvelopeModel
    {
        public DocumentType Type { get; set; }

        public UserDetails AdditionalUser { get; set; }

        public string RedirectUrl { get; set; }
    }
}
