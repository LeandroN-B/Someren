using Someren.Models;

namespace Someren.Repositories
{
    public interface ILecturerRepository
    {
        List<Lecturer> GetAllLecturers();
        Lecturer? GetLecturerByID(int id);
        void AddLecturer(Lecturer lecturer);
        void UpdateLecturer(Lecturer lecturer);
        void DeleteLecturer(Lecturer lecturer);
    }
}
