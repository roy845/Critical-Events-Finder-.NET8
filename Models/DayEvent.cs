using System.ComponentModel.DataAnnotations;

namespace Critical_Events_Finder_Api.Models
{
    public class DayEvent
    {
        [Required]
        public string Intersection { get; set; } = string.Empty;
        [Required]
        public string Event { get; set; } = string.Empty;

    }
}
