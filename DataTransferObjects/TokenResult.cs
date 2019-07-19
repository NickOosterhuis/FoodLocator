using System;
using Newtonsoft.Json;

namespace DTO
{
    public class TokenResult
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public uint ExpiresIn { get; set; }

        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }

        [JsonProperty(PropertyName = ".issued")]
        public DateTimeOffset Issued { get; set; }

        [JsonProperty(PropertyName = ".expires")]
        public DateTimeOffset Expires { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
