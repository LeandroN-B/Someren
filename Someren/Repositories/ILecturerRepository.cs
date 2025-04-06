namespace Someren.Repositories
{
    using Someren.Models;

    public interface ILecturerRepository
    {
        List<Lecturer> GetAllLecturers(string lastName = ""); //This is for index, and Read in CRUD
        Lecturer? GetLecturerByID(int id);//Used for edit and delete
        void AddLecturer(Lecturer lecturer); // Create in CRUD
        void UpdateLecturer(Lecturer lecturer);// Edit in CRUD
        void DeleteLecturer(Lecturer lecturer);// Delete in CRUD      



        // Supervisor (Activity-Lecturer) management 
        public List<Lecturer> GetSupervisorsForActivity(int activityId);

        public List<Lecturer> GetNonSupervisorsForActivity(int activityId);

        public void AddSupervisor(int activityId, int lecturerId);

        public void RemoveSupervisor(int activityId, int lecturerId);

    }
}
