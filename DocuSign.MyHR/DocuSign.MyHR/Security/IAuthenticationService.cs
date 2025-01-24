using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace DocuSign.MyHR.Security
{
    public interface IAuthenticationService
    { 
        (ClaimsPrincipal , AuthenticationProperties) AuthenticateFromJwt(); 
    }
}