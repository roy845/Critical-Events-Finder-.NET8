using Amazon.S3;
using Critical_Events_Finder_Api.Models;
using Critical_Events_Finder_Api.Models.FileUploadAPI.Models;

namespace Critical_Events_Finder_Api.Interfaces
{
    public interface IFileUploadService
    {
        Task<FileUploadResponse> CreateBucket(string bucketName);
        Task<FileUploadResponse> UploadExcelFile(IFormFile file);
       Task<FileUploadResponse> UploadJSONFile(IFormFile file);
       Task<ListAllFilesResponse> ListFiles(int page = 1, int limit = 10, string search = "");
       Task<FileUploadResponse> DeleteFile(string fileName);
       Task<FileUploadResponse> DeleteAllFiles();
       Task<FileUploadResponse> CreateFolder(string folderName);
       Task<FileUploadResponse> DownloadAndProcessFile(string file_name, string file_type);
    }
}
