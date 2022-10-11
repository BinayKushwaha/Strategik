using System.Threading.Tasks;

namespace Strategik.Tool.Services.Drive
{
    public interface IDriveService
    {
        Task<int> UploadFilesAsync(string sourcePath);
    }
}
