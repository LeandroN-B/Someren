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

        Lecturer? GetLecturerByRoomID(int roomId);
        bool IsRoomAvailableForLecturer(int roomId);
        void AssignRoom(int lecturerId, int roomId);
    }
}
