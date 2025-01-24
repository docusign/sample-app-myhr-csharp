namespace DocuSign.MyHR.Services
{
    public class CreateEnvelopeResponse
    {
        public CreateEnvelopeResponse(string redirectUrl, string envelopeId)
        {
            RedirectUrl = redirectUrl;
            EnvelopeId = envelopeId;
        }

        public string RedirectUrl { get; }
        public string EnvelopeId { get; }
    }
}
