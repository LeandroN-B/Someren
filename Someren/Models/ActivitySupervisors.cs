namespace Someren.Models
{
    public class ActivitySupervisors
    {
        public Activity Activity { get; set; }
        public List<Lecturer> Supervisors { get; set; } = new();
        public List<Lecturer> NonSupervisors { get; set; } = new();

        public ActivitySupervisors() { }//ctor parameterless

    }
}
