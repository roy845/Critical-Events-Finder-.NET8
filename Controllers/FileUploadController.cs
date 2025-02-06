using Microsoft.AspNetCore.Mvc;
using Critical_Events_Finder_Api.Models;
using Critical_Events_Finder_Api.Interfaces;
using Critical_Events_Finder_Api.Models.FileUploadAPI.Models;


namespace Critical_Events_Finder_Api.Controllers
{
    [ApiController]
    [Route("api/file-upload")]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;
     
        public FileUploadController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
         
        }

        // Upload Excel file
        [HttpPost("uploadExcel")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            var result = await _fileUploadService.UploadExcelFile(file);
            return StatusCode(result.StatusCode, result);
        }

        // Upload JSON file
        [HttpPost("uploadJSON")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadJSON(IFormFile file)
        {
            var result = await _fileUploadService.UploadJSONFile(file);
            return StatusCode(result.StatusCode, result);
        }

        // List Files with Pagination and Search
        [HttpGet("listFiles")]
        public async Task<IActionResult> ListFiles(int page = 1, int limit = 10, string search = "")
        {
            var result = await _fileUploadService.ListFiles(page, limit, search);
            return StatusCode(result.StatusCode, result);
        }

        // Delete a File
        [HttpDelete("deleteFile/{file_name}")]
        public async Task<IActionResult> DeleteFile(string file_name)
        {
            var result = await _fileUploadService.DeleteFile(file_name);
            return StatusCode(result.StatusCode, result);
        }

        // Delete All Files
        [HttpDelete("deleteAllFiles")]
        public async Task<IActionResult> DeleteAllFiles()
        {
            var result = await _fileUploadService.DeleteAllFiles();
            return StatusCode(result.StatusCode, result);
        }

        // Create Folder in S3
        [HttpPost("createFolder")]
        public async Task<IActionResult> CreateFolder([FromBody] CreateFolderRequest request)
        {
          

            var result = await _fileUploadService.CreateFolder(request.folder_name);
            return StatusCode(result.StatusCode, result);
        }

        // Download and Process File
        [HttpGet("downloadAndProcessFile/{file_name}")]
        public async Task<IActionResult> DownloadAndProcessFile(string file_name, string file_type)
        {
            var result = await _fileUploadService.DownloadAndProcessFile(file_name, file_type);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("createBucket")]
        public async Task<IActionResult> CreateBucket([FromBody] CreateBucketRequest request)
        {
            var result = await _fileUploadService.CreateBucket(request.BucketName);
            return StatusCode(result.StatusCode, result);
        }
    }
}
