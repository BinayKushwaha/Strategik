using Microsoft.Extensions.Hosting;
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
        public StrategikWorker(FileWatcher fileWatcher)
        {
            this.fileWatcher = fileWatcher;
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
            var archivePath = System.Configuration.ConfigurationManager.AppSettings["ArchivePath"];
            if (!string.IsNullOrEmpty(archivePath))
            {
                var successDirectory=archivePath+$"\\{Archive.Success.ToString()}";
                var failDirectory=archivePath+$"\\{Archive.Fail.ToString()}";

                var paths = new List<string>();
                paths.Add(successDirectory);
                paths.Add(failDirectory);

                DirectoryHelper.CreateDirectories(paths);
            }
        }
    }
}
