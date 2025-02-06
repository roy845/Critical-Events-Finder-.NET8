using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Critical_Events_Finder_Api.Interfaces;
using Critical_Events_Finder_Api.Models;
using Critical_Events_Finder_Api.Models.FileUploadAPI.Models;
using Critical_Events_Finder_Api.Utilities;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System.Text;

namespace Critical_Events_Finder_Api.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public FileUploadService(IConfiguration configuration)
        {
          
            _bucketName = configuration["AWS:BucketName"];

            _s3Client = new AmazonS3Client(
                configuration["AWS:AccessKey"],
                configuration["AWS:SecretKey"],
                RegionEndpoint.GetBySystemName(configuration["AWS:Region"])
            );
        }

        // 📌 Upload Excel File
        public async Task<FileUploadResponse> UploadExcelFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new FileUploadResponse { message = "No file provided.", StatusCode = 400 };

            if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
                return new FileUploadResponse { message = "Invalid file type. Only Excel files allowed.", StatusCode = 400 };

            var key = $"royatali/{file.FileName}";

            using var stream = file.OpenReadStream();
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = stream
            };

            await _s3Client.PutObjectAsync(putRequest);

            return new FileUploadResponse { message = "Excel file uploaded successfully.", StatusCode = 200 };
        }

        // 📌 Upload JSON File
        public async Task<FileUploadResponse> UploadJSONFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new FileUploadResponse { message = "No file provided.", StatusCode = 400 };

            if (!file.FileName.EndsWith(".json"))
                return new FileUploadResponse { message = "Invalid file type. Only JSON files allowed.", StatusCode = 400 };

            using var reader = new StreamReader(file.OpenReadStream());
            var jsonContent = await reader.ReadToEndAsync();

            // Validate JSON Schema
            if (!JsonSchemaValidator.ValidateJson(jsonContent))
                return new FileUploadResponse { message = "Invalid JSON structure.", StatusCode = 400 };

            var key = $"royatali/{file.FileName}";

            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent));
            await _s3Client.UploadObjectFromStreamAsync(_bucketName, key, memoryStream, null);

            return new FileUploadResponse { message = "JSON file uploaded successfully.", StatusCode = 200 };
        }

        // 📌 List Files in S3 Bucket (with pagination and search)
        public async Task<ListAllFilesResponse> ListFiles(int page = 1, int limit = 10, string search = "")
        {
            try
            {

                var request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = "royatali/"
                };

                var response = await _s3Client.ListObjectsV2Async(request);
                var allFiles = response.S3Objects
                    .Where(obj => !obj.Key.EndsWith("/"))  // Exclude directories
                    .Select(obj => new { FileName = obj.Key, Size = obj.Size })
                    .ToList();

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    allFiles = allFiles.Where(f => f.FileName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Pagination
                int totalFiles = allFiles.Count;
                int totalPages = (int)Math.Ceiling((double)totalFiles / limit);
                var paginatedFiles = allFiles
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .Select(f => new FileDetails { file_name = f.FileName, Size = f.Size })
                    .ToList();

                return new ListAllFilesResponse
                {
                    message = "Files listed successfully.",
                    StatusCode = 200,
                    total_files = totalFiles,
                    page = page,
                    limit = limit,
                    total_pages = totalPages,
                    files = paginatedFiles
                };
            }
            catch (Exception ex)
            {
                return new ListAllFilesResponse { message = "Failed to list files", StatusCode = 500, Data = ex.Message };
            }
        }

        // 📌 Delete a Specific File
        public async Task<FileUploadResponse> DeleteFile(string fileName)
        {
            try
            {
                var key = $"royatali/{fileName}";
                await _s3Client.DeleteObjectAsync(_bucketName, key);

                return new FileUploadResponse { message = $"File {fileName} deleted successfully.",StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new FileUploadResponse { message = $"Failed to delete {fileName}.", StatusCode = 500, Data = ex.Message };
            }
        }

        // 📌 Delete All Files
        public async Task<FileUploadResponse> DeleteAllFiles()
        {
            try
            {
                var listRequest = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = "royatali/"
                };

                var listResponse = await _s3Client.ListObjectsV2Async(listRequest);
                if (!listResponse.S3Objects.Any())
                    return new FileUploadResponse { message = "No files found.", StatusCode = 200 };

                var deleteRequest = new DeleteObjectsRequest
                {
                    BucketName = _bucketName,
                    Objects = listResponse.S3Objects.Select(o => new KeyVersion { Key = o.Key }).ToList()
                };

                var deleteResponse = await _s3Client.DeleteObjectsAsync(deleteRequest);

                return new FileUploadResponse
                {
                    message = "All files deleted successfully.",
                    StatusCode = 200,
                    Data = new { DeletedFiles = deleteResponse.DeletedObjects.Count }
                };
            }
            catch (Exception ex)
            {
                return new FileUploadResponse { message = "Failed to delete all files.", StatusCode = 500, Data = ex.Message };
            }
        }

        // 📌 Create a Folder
        public async Task<FileUploadResponse> CreateFolder(string folderName)
        {
            try
            {
                if (!folderName.EndsWith("/"))
                    folderName += "/";

                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = $"royatali/{folderName}",
                    ContentBody = string.Empty
                };

                await _s3Client.PutObjectAsync(putRequest);
                return new FileUploadResponse { message = $"Folder '{folderName}' created successfully.", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new FileUploadResponse { message = $"Failed to create folder '{folderName}'.", StatusCode = 500, Data = ex.Message };
            }
        }

        // 📌 Download and Process File
        public async Task<FileUploadResponse> DownloadAndProcessFile(string file_name, string file_type)
        {
            try
            {
                var key = $"royatali/{file_name}";

                var response = await _s3Client.GetObjectAsync(_bucketName, key);
                using var stream = response.ResponseStream;
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var fileContent = memoryStream.ToArray();

                // 📌 Process JSON File
                if (file_type.Equals("json", StringComparison.OrdinalIgnoreCase))
                {
                    string jsonString = Encoding.UTF8.GetString(fileContent);

                    try
                    {
                        var jsonData = JArray.Parse(jsonString);

                        var daysList = jsonData.Select(day => new
                        {
                            id = day["id"]?.ToString(),
                            events = day["events"]?.Select(e => new
                            {
                                intersection = e["intersection"]?.ToString(),
                                @event = e["event"]?.ToString()
                            }).ToList()
                        }).ToList();

                        return new FileUploadResponse
                        {
                            message = "File processed successfully",
                            StatusCode = 200,
                            days_list = daysList
                        };
                    }
                    catch (Exception ex)
                    {
                        return new FileUploadResponse
                        {
                            message = "Invalid JSON structure.",
                            StatusCode = 400,
                            Data = ex.Message
                        };
                    }
                }

                // 📌 Process Excel File
                if (file_type.Equals("xlsx", StringComparison.OrdinalIgnoreCase) || file_type.Equals("xls", StringComparison.OrdinalIgnoreCase))
                {
                    using var excelStream = new MemoryStream(fileContent);
                    using var package = new ExcelPackage(excelStream);
                    var worksheet = package.Workbook.Worksheets[0];

                    if (worksheet == null)
                        return new FileUploadResponse { message = "Excel file is empty.", StatusCode = 400 };

                    var daysDict = new Dictionary<string, dynamic>(); ;
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        string dayId = worksheet.Cells[row, 1].Text;
                        string intersection = worksheet.Cells[row, 2].Text;
                        string eventName = worksheet.Cells[row, 3].Text;

                        string formattedDayId = $"day-{dayId}";

                        if (!daysDict.ContainsKey(formattedDayId))
                        {
                            daysDict[formattedDayId] = new
                            {
                                id = formattedDayId,
                                events = new List<object>()
                            };
                        }

                        // Add event to the existing day
                        ((List<object>)daysDict[formattedDayId].events).Add(new { intersection = intersection, @event = eventName });
                    }

                    return new FileUploadResponse { message = "File processed successfully.", StatusCode = 200, days_list = daysDict.Values.ToList() };
                }

                return new FileUploadResponse { message = "Unsupported file type.",StatusCode = 400 };
            }
            catch (Exception ex)
            {
                return new FileUploadResponse { message = "Error processing file.", StatusCode = 500, Data = ex.Message };
            }
        }

        public async Task<FileUploadResponse> CreateBucket(string bucketName)
        {
            try
            {
                var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
                    if (bucketExists)
                        return new FileUploadResponse { message = $"Bucket '{bucketName}' already exists.", StatusCode = 400 };

                var request = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                };

                await _s3Client.PutBucketAsync(request);

                return new FileUploadResponse { message = $"Bucket '{bucketName}' created successfully.", StatusCode = 200 };
            }
            catch (AmazonS3Exception s3Ex)
            {
                return new FileUploadResponse
                {
                    StatusCode = (int)s3Ex.StatusCode,
                    message = $"AWS S3 Error: {s3Ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new FileUploadResponse
                {
                    StatusCode = 500,
                    message = $"Failed to create bucket '{bucketName}'.",
                    Data = ex.Message
                };
            }
        }
    }
}
