namespace Critical_Events_Finder_Api.Models
{
    public class ListAllFilesResponse
    {
        public string message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public int total_files { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public int total_pages { get; set; }
        public List<FileDetails> files { get; set; } = new List<FileDetails>();
        public object? Data { get; set; } = null;
    }

    public class FileDetails
    {
        public string file_name { get; set; } = string.Empty;
        public long Size { get; set; }
    }
}
