using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Strategik.Tool.Authentication;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Strategik.Tool.Services.Client
{
    public class GraphClientService : IGraphClientService
    {
        private readonly IAuthHandler authHandler;
        private readonly ILogger<GraphClientService> logger;
        public GraphClientService(IAuthHandler authHandler, ILogger<GraphClientService> logger)
        {
            this.authHandler = authHandler;
            this.logger = logger;
        }
        public async Task<GraphServiceClient> GetClientAsync()
        {
            try
            {

                var token = await this.authHandler.GetTokenAsync();


                logger.LogInformation("Start creating Graph Client.");

                var graphClient = new GraphServiceClient(
                    new DelegateAuthenticationProvider(
                        requestMessage =>
                        {
                        // Append the access token to the request.
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

                        // Get event times in the current time zone.
                        requestMessage.Headers.Add("Prefer", "outlook.timezone=\"" + TimeZoneInfo.Local.Id + "\"");

                            return Task.CompletedTask;
                        }));

                logger.LogInformation("Completed creating Graph Client.");

                return graphClient;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
