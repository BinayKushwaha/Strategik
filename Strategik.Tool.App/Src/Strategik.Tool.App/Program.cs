using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Strategik.Tool.Authentication;
using Strategik.Tool.Services.Client;
using Strategik.Tool.Services.Drive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategik.Tool.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);
            builder.ConfigureServices(
                services =>
                    services.AddHostedService<StrategikWorker>()
                    .AddSingleton<FileWatcher>()
                    .AddTransient<IGraphClientService, GraphClientService>()
                        .AddTransient<IDriveService, DriveService>()
                        .AddHttpClient<IAuthHandler, AuthHandler>());

            var host = builder.Build();

            host.Run();
        }
    }
}
