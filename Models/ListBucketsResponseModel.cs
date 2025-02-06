namespace Critical_Events_Finder_Api.Models
{
    public class ListBucketsResponseModel
    {
        public int StatusCode { get; set; }
        public string message { get; set; } = string.Empty;
        public List<BucketDetails> Buckets { get; set; } = new();
        public string Data { get; set; } = string.Empty;
    }
}
