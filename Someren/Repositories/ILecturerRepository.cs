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
      
    }
}
