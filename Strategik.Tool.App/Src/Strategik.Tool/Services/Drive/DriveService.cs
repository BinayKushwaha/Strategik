using Microsoft.Graph;
using Strategik.Tool.Services.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Strategik.Tool.Services.Drive
{
    public class DriveService : IDriveService
    {
        public readonly IGraphClientService graphClientService;
        public DriveService(IGraphClientService graphClientService)
        {
            this.graphClientService = graphClientService;
        }
        public async Task<int> UploadFilesAsync(string sourcePath)
        {
            try
            {
                using (var fileStream =new  FileStream(sourcePath,FileMode.Open,FileAccess.Read))
                {
                    // Use properties to specify the conflict behavior
                    // in this case, replace
                    var uploadProps = new DriveItemUploadableProperties
                    {
                        AdditionalData = new Dictionary<string, object>
                        {
                            { "@microsoft.graph.conflictBehavior", "replace" }
                        }
                    };

                    var graphClient =await graphClientService.GetClientAsync();

                    // Create the upload session
                    // itemPath does not need to be a path to an existing item
                    var uploadSession = await graphClient.Sites["strategikoffice365.sharepoint.com"].Drive.Root
                        .ItemWithPath("/demofile")
                        .CreateUploadSession(uploadProps)
                        .Request()
                        .PostAsync();

                    // Max slice size must be a multiple of 320 KiB
                    int maxSliceSize = 320 * 1024;
                    var fileUploadTask =
                        new LargeFileUploadTask<DriveItem>(uploadSession, fileStream, maxSliceSize);

                    var totalLength = fileStream.Length;
                    // Create a callback that is invoked after each slice is uploaded
                    IProgress<long> progress = new Progress<long>(prog => {
                        Console.WriteLine($"Uploaded {prog} bytes of {totalLength} bytes");
                    });


                    try
                    {
                        // Upload the file
                        var uploadResult = await fileUploadTask.UploadAsync(progress);

                        Console.WriteLine(uploadResult.UploadSucceeded ?
                            $"Upload complete, item ID: {uploadResult.ItemResponse.Id}" :
                            "Upload failed");
                    }
                    catch (ServiceException ex)
                    {
                        Console.WriteLine($"Error uploading: {ex.ToString()}");
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
