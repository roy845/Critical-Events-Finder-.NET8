using System.Collections.Concurrent;
using Critical_Events_Finder_Api.Models;

namespace Critical_Events_Finder_Api.Interfaces
{
    public interface ICriticalEventsService
    {
        Dictionary<string, HashSet<string>> UpdateEventIntersections(List<DayEvent> day);
        void UpdateEventDaysCount(
            Dictionary<string, HashSet<string>> eventIntersections,
            ConcurrentDictionary<string, int> eventDaysCount,
            HashSet<string> criticalEvents);
        (List<string>, int, string) FindCriticalEvents(List<List<DayEvent>> days_list);


    }
}
