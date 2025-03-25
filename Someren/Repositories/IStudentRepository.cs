using System.Collections.Generic;
using Someren.Models;

namespace Someren.Repositories
{
    public interface IStudentRepository
    {
        List<Student> GetAllStudents();
        Student? GetStudentByID(int id);
        void AddStudent(Student student);
        void UpdateStudent(Student student);
        void DeleteStudent(Student student);
        List<Student> GetStudentsByRoomID(int roomId);
      //  List<Student> GetStudentsByLastName(string lastName);
    }
}
