namespace Critical_Events_Finder_Api.Models
{
    public class DaysListRequestFromFile
    {
        public string Id { get; set; }
        public List<DayEvent> Events { get; set; } = new List<DayEvent>();
    }
}
