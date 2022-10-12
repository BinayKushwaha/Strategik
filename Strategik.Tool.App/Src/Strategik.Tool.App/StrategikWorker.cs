using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Strategik.Tool.Enum;
using Strategik.Tool.Helper;
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
        private readonly ILogger<StrategikWorker> logger;
        public StrategikWorker(FileWatcher fileWatcher, ILogger<StrategikWorker> logger)
        {
            this.fileWatcher = fileWatcher;
            this.logger = logger;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            CreateArchiveDirectory();
            await ExecuteAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                fileWatcher.WatchFiles();
            }
        }
        private void CreateArchiveDirectory()
        {
            logger.LogInformation("Start creating Archive directory.");

            var archivePath = System.Configuration.ConfigurationManager.AppSettings["ArchivePath"];
            if (!string.IsNullOrEmpty(archivePath))
            {
                var successDirectory=archivePath+$"\\{Archive.Success.ToString()}";
                var failDirectory=archivePath+$"\\{Archive.Fail.ToString()}";

                var paths = new List<string>();
                paths.Add(successDirectory);
                paths.Add(failDirectory);

                DirectoryHelper.CreateDirectories(paths);

                logger.LogInformation("Completed creating Archive directory.");
            }
            else
            {
                logger.LogError("Failed creating Archive directory as the ArchivePath is null.");
            }
        }
    }
}
