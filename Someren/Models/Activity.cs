using System.ComponentModel.DataAnnotations;

namespace Someren.Models
{
    public class Activity
    {
        public int ActivityID { get; set; }//PK
        
        public string ActivityName { get; set; } = string.Empty;

        public DateTime ActivityDate { get; set; }
        
        public string TimeOfDay { get; set; } = string.Empty; // Morning / Afternoon 

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int DurationMinutes //calculated property
        {
            get
            {
                return (int)(EndTime - StartTime).TotalMinutes;
            }
        }
        public Activity() { } //ctor parameterless

        public Activity(int id, string name, DateTime date, string timeOfDay, TimeSpan startTime, TimeSpan endTime)
        {
            ActivityID = id;
            ActivityName = name;
            ActivityDate = date;
            TimeOfDay = timeOfDay;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
