namespace Someren.Repositories
{
    using Someren.Models;

    public interface ILecturerRepository
    {
        List<Lecturer> GetAllLecturers(); //This is for index, and Read in CRUD
        Lecturer? GetLecturerByID(int id);//Used for edit and delete
        void AddLecturer(Lecturer lecturer); // Create in CRUD
        void UpdateLecturer(Lecturer lecturer);// Edit in CRUD
        void DeleteLecturer(Lecturer lecturer);// Delete in CRUD
        List<Lecturer> GetLecturersByLastName(string lastName);//Order list by last name
        Lecturer? GetLecturerByRoomID(int roomId);
        bool IsRoomFreeForAddLecturer(int roomId);//The logic to assigne a room in Lecturer's creation
        bool IsRoomFreeForEditLecturer(int lecturerId, int newRoomId);//The logic that allows edition in Lecturer's room

      
    }
}
