using Someren.Models;
using System.Collections.Generic;
using System.Linq;

public class StudentRepository : IStudentRepository
{
    private readonly List<Student> _students;

    public StudentRepository()
    {
        // Dummy data (replace with DB connection)
        _students = new List<Student>
        {
            new Student { StudentId = 1, FirstName = "Alice", LastName = "Brown", Telephone = "06-11111111", Class = "CS101" },
            new Student { StudentId = 2, FirstName = "Bob", LastName = "Smith", Telephone = "06-22222222", Class = "CS101" },
            new Student { StudentId = 3, FirstName = "Charlie", LastName = "Jones", Telephone = "06-33333333", Class = "CS101" }
            // Add more students as needed
        };
    }

    public List<Student> GetAllStudents() => _students.OrderBy(s => s.LastName).ToList();

    public Student? GetStudentByID(int id) => _students.FirstOrDefault(s => s.StudentId == id);

    public void AddStudent(Student student) => _students.Add(student);

    public void UpdateStudent(Student student)
    {
        var existing = _students.FirstOrDefault(s => s.StudentId == student.StudentId);
        if (existing != null)
        {
            existing.FirstName = student.FirstName;
            existing.LastName = student.LastName;
            existing.Telephone = student.Telephone;
            existing.Class = student.Class;
        }
    }

    public void DeleteStudent(Student student) => _students.Remove(student);
}
