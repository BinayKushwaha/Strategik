using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Strategik.Tool.App
{
    internal class StrategikWorker : BackgroundService
    {
        private readonly FileWatcher fileWatcher;
        public StrategikWorker(FileWatcher fileWatcher)
        {
            this.fileWatcher = fileWatcher;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await ExecuteAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                fileWatcher.WatchFiles();
            }
        }
    }
}
