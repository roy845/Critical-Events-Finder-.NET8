using System.ComponentModel.DataAnnotations;

namespace Critical_Events_Finder_Api.Models
{
    public class DaysListRequest
    {
        [Required]
        public List<List<DayEvent>> days_list { get; set; } = new List<List<DayEvent>>();
    }
}
