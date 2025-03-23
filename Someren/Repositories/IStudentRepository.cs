using Someren.Models;

public interface IStudentRepository
{
    List<Student> GetAllStudents();
    Student? GetStudentByID(int id);
    void AddStudent(Student student);
    void UpdateStudent(Student student);
    void DeleteStudent(Student student);
    List<Student> GetStudentsByRoomID(int roomId); 
}
