using System;
using System.Linq;
using System.Security.Claims;
using DocuSign.MyHR.Domain; 
using Newtonsoft.Json;

namespace DocuSign.MyHR
{
    public class Context
    { 
        public static User User { get; private set; }
        public static Account Account { get; private set; }

        public void Init(ClaimsPrincipal principalUser)
        {
            var userId = principalUser.FindFirstValue(ClaimTypes.NameIdentifier);
            User = new User(
                userId,
                principalUser.FindFirstValue(ClaimTypes.Name),
                Enum.Parse<LoginType>(principalUser.FindFirstValue("authType"))
            );
            Account = principalUser.FindAll("accounts").Select(x => JsonConvert.DeserializeObject<Account>(x.Value))
                .First(x => x.Id == principalUser.FindFirstValue("account_id")); 
        } 
    }
}
