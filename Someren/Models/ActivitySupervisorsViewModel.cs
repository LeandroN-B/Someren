namespace Someren.Models
{
    public class ActivitySupervisorsViewModel
    {
        public Activity Activity { get; set; }
        public List<Lecturer> Supervisors { get; set; } = new();
        public List<Lecturer> NonSupervisors { get; set; } = new();

        public ActivitySupervisorsViewModel() { }//ctor parameterless

    }
}
