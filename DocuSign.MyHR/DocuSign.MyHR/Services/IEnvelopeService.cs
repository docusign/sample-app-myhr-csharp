using System.Collections.Generic;
using DocuSign.MyHR.Domain;

namespace DocuSign.MyHR.Services
{
    public interface IEnvelopeService
    {
        CreateEnvelopeResponse CreateEnvelope(
            DocumentType type, 
            string accountId,
            string userId,
            LoginType loginType,
            UserDetails additionalUser,
            string redirectUrl,
            string pingAction);

        Dictionary<string, string> GetEnvelopData(
            string accountId,
            string envelopeId);
    }
}