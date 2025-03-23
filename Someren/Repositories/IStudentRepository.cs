using Someren.Models;

public interface IStudentRepository
{
    List<Student> GetAllStudents();
    Student? GetStudentByID(int id);
    void AddStudent(Student student);
    void UpdateStudent(Student student);
    void DeleteStudent(Student student);
    // New method to get students by room id
    List<Student> GetStudentsByRoomID(int roomId);
}
