using Microsoft.Extensions.Logging;
using Strategik.Tool.Enum;
using Strategik.Tool.Services.Drive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategik.Tool.App
{
    public class FileWatcher
    {
        private string SourceDirectory { get; set; }
        private string FileFilter { get; set; }
        private string SuccessDirectory { get; set; }
        private string FailDirectory { get; set; }

        private readonly ILogger<FileWatcher> logger;
        private readonly IDriveService driveService;
        public FileWatcher(IDriveService driveService, ILogger<FileWatcher> logger)
        {
            SourceDirectory = System.Configuration.ConfigurationManager.AppSettings["SourceDirectory"];
            FileFilter = System.Configuration.ConfigurationManager.AppSettings["FileFilter"];
            SuccessDirectory = System.Configuration.ConfigurationManager.AppSettings["ArchivePath"] + $"\\{Archive.Success.ToString()}";
            FailDirectory = System.Configuration.ConfigurationManager.AppSettings["ArchivePath"] + $"\\{Archive.Fail.ToString()}";

            this.driveService = driveService;
            this.logger = logger;
        }
        public void WatchFiles()
        {
            using (var watcher = new FileSystemWatcher(SourceDirectory))
            {

                watcher.NotifyFilter = NotifyFilters.Attributes
                                     | NotifyFilters.CreationTime
                                     | NotifyFilters.DirectoryName
                                     | NotifyFilters.FileName
                                     | NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.Security
                                     | NotifyFilters.Size;

                watcher.Created += OnCreated;
                watcher.Deleted += OnDeleted;
                watcher.Error += OnError;

                watcher.Filter = FileFilter;
                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;

                //Console.WriteLine("Press enter to exit.");
                Console.WriteLine("Strategik App.");
                Console.ReadLine();
            }
        }
        private async void OnCreated(object sender, FileSystemEventArgs e)
        {
            string fileName = Path.GetFileName(e.FullPath);

            string value = $"Created: {e.FullPath}";
            logger.LogInformation(value);

            var result = await driveService.UploadFilesAsync(e.FullPath);

            if (result > 0)
            {
                string destinationPath = SuccessDirectory + "\\" + fileName;
                File.Move(e.FullPath, destinationPath);

                logger.LogInformation($"File: {fileName} successfully uploaded and moved to directory: {destinationPath}");
            }
            else
            {
                string destinationPath = FailDirectory + "\\" + fileName;
                File.Move(e.FullPath, destinationPath);

                logger.LogError($"File: {fileName} failed to upload and moved to directory: {destinationPath}");
            }
        }

        private void OnDeleted(object sender, FileSystemEventArgs e) =>
            logger.LogInformation($"Deleted: {e.FullPath}");

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}
