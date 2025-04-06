namespace Someren.Models
{
    public class ActivityParticipantsViewModel
    {
        public int ActivityID { get; set; }
        public Activity Activity { get; set; }
        public List<Student> Participants { get; set; }
        public List<Student> NonParticipants { get; set; }
        public string ConfirmationMessage { get; set; }


        public ActivityParticipantsViewModel()
        {
            Activity = new Activity();
            Participants = new List<Student>();
            NonParticipants = new List<Student>();
            ConfirmationMessage = string.Empty;
        }
    }
}