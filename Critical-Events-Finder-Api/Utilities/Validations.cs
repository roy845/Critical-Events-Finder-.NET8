using Critical_Events_Finder_Api.Models;

namespace Critical_Events_Finder_Api.Utilities
{
    public static class Validations
    {
        public static bool IsValidDaysList(List<List<DayEvent>> daysList)
        {
            if (daysList == null || daysList.Count == 0)
                return false;

            foreach (var day in daysList)
            {
                if (day == null || day.Count == 0)
                    return false;

                foreach (var entry in day)
                {
                    if (entry == null || string.IsNullOrWhiteSpace(entry.Intersection) || string.IsNullOrWhiteSpace(entry.Event))
                        return false;
                }
            }

            return true;
        }
    }
}
