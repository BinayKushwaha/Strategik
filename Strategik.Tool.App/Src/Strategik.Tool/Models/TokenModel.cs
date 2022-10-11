using Newtonsoft.Json;

namespace Strategik.Tool.Models
{
    public class TokenModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
