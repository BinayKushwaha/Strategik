using Microsoft.Graph;
using System.Threading.Tasks;

namespace Strategik.Tool.Services.Client
{
    public interface IGraphClientService
    {
        Task<GraphServiceClient> GetClientAsync();
    }
}
