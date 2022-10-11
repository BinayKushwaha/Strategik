﻿using Strategik.Tool.Services.Drive;
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
        private readonly IDriveService driveService;
        public FileWatcher(IDriveService driveService)
        {
            SourceDirectory = System.Configuration.ConfigurationManager.AppSettings["SourceDirectory"];
            this.driveService = driveService;
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

                watcher.Changed += OnChanged;
                watcher.Created += OnCreated;
                watcher.Deleted += OnDeleted;
                watcher.Error += OnError;

                watcher.Filter = "*";
                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;

                //Console.WriteLine("Press enter to exit.");
                Console.WriteLine("Strategik App.");
                Console.ReadLine();
            }
        }
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        private async void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
            SourceDirectory = SourceDirectory + "\\StrategikFile.txt";
            await driveService.UploadFilesAsync(SourceDirectory);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

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