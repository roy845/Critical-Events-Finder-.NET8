namespace Critical_Events_Finder_Api.Models
{
    namespace FileUploadAPI.Models
    {
        public class FileUploadResponse
        {
            public string message { get; set; } = string.Empty;
            public int StatusCode { get; set; }
            public object? Data { get; set; } = null;

            public object? days_list { get; set; } = null;

        }
    }

}
