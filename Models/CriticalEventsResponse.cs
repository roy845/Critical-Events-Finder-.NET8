namespace Critical_Events_Finder_Api.Models
{
    public class CriticalEventsResponse
    {
        public List<string> critical_events { get; set; } = new List<string>();
        public int status_code { get; set; }
        public string message { get; set; } = string.Empty;
    }
}
