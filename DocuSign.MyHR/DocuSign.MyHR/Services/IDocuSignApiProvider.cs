using System.Net.Http;
using DocuSign.eSign.Api;

namespace DocuSign.MyHR.Services
{
    public interface IDocuSignApiProvider
    {
        IUsersApi UsersApi { get; }
        IEnvelopesApi EnvelopApi { get; }
        HttpClient DocuSignHttpClient { get; }
        ITemplatesApi TemplatesApi { get; }
        IAccountsApi AccountsApi { get; }
    }
}