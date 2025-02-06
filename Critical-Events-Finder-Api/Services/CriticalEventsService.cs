using System.Collections.Concurrent;
using Critical_Events_Finder_Api.Interfaces;
using Critical_Events_Finder_Api.Models;
using Critical_Events_Finder_Api.Utilities;

namespace Critical_Events_Finder_Api.Services
{
    public class CriticalEventsService : ICriticalEventsService
    {
        private const int MinDays = 2;
        private const int MinIntersections = 2;
        public Dictionary<string, HashSet<string>> UpdateEventIntersections(List<DayEvent> day)
        {
            var eventIntersections = new Dictionary<string, HashSet<string>>();

            foreach (var entry in day)
            {
                if (!eventIntersections.ContainsKey(entry.Event))
                    eventIntersections[entry.Event] = new HashSet<string>();

                eventIntersections[entry.Event].Add(entry.Intersection);
            }

            return eventIntersections;
        }

        public void UpdateEventDaysCount(Dictionary<string, HashSet<string>> eventIntersections, ConcurrentDictionary<string, int> eventDaysCount, HashSet<string> criticalEvents)
        {
            foreach (var kvp in eventIntersections)
            {
                if (kvp.Value.Count >= MinIntersections)
                {
                    eventDaysCount.AddOrUpdate(kvp.Key, 1, (_, count) => count + 1);

                    if (eventDaysCount[kvp.Key] >= MinDays)
                        criticalEvents.Add(kvp.Key);
                }
            }
        }

        public (List<string>, int, string) FindCriticalEvents(List<List<DayEvent>> days_list)
        {
            if (!Validations.IsValidDaysList(days_list))
                return (new List<string>(), StatusCodes.Status404NotFound, "Days list is invalid");

            var eventDaysCount = new ConcurrentDictionary<string, int>();
            var criticalEvents = new HashSet<string>();

            foreach (var day in days_list)
            {
                var eventIntersections = UpdateEventIntersections(day);
                UpdateEventDaysCount(eventIntersections, eventDaysCount, criticalEvents);
            }

            return (criticalEvents.ToList(), StatusCodes.Status200OK, "Critical events found");
        }

        

        
    }
}
