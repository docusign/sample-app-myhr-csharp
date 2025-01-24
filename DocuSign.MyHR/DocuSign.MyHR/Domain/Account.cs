using System;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Domain
{
    [Serializable]
    public class Account
    {
        public Account()
        {
        }
        
        public Account(string id, string name, string baseUri)
        {
            Id = id;
            Name = name;
            BaseUri = baseUri;
        }

        [JsonProperty("account_id")]
        public string Id { get; set; }

        [JsonProperty("account_name")]
        public string Name { get; set; }
        
        [JsonProperty("base_uri")]
        public string BaseUri { get; set; }
    }
}