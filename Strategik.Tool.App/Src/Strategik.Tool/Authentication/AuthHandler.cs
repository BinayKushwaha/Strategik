using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Strategik.Tool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Strategik.Tool.Authentication
{
    public class AuthHandler : IAuthHandler
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<AuthHandler> logger;

        public AuthHandler(HttpClient httpClient, ILogger<AuthHandler> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger; 
        }

        public async Task<string> GetTokenAsync()
        {
            try
            {
                logger.LogInformation("Start fetching token.");

                var appId = System.Configuration.ConfigurationManager.AppSettings["AppId"];
                var appSecrect = System.Configuration.ConfigurationManager.AppSettings["ClientSecrect"];
                var tenantId = System.Configuration.ConfigurationManager.AppSettings["TenantID"];

                string response = await POST($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token",
                      $"grant_type=client_credentials&client_id={appId}&client_secret={appSecrect}"
                      + "&scope=https%3A%2F%2Fgraph.microsoft.com%2F.default");

                var token = JsonConvert.DeserializeObject<TokenModel>(response).AccessToken;
                
                logger.LogInformation("Completed fetching token.");

                return token;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> POST(string url, string body)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                HttpResponseMessage response = await httpClient.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception(responseBody);
                return responseBody;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
