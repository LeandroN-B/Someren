namespace Someren.Repositories
{
    using Someren.Models;

    public interface ILecturerRepository
    {
        List<Lecturer> GetAllLecturers();
        Lecturer? GetLecturerByID(int id);
        void AddLecturer(Lecturer lecturer);
        void UpdateLecturer(Lecturer lecturer);
        void DeleteLecturer(Lecturer lecturer);
        List<Lecturer> GetLecturersInRoom(int roomId);


    }
}
