using System.Collections.Generic;
using Someren.Models;

namespace Someren.Repositories
{
    public interface IStudentRepository
    {
        List<Student> GetAllStudents(string lastName = "");
        Student? GetStudentByID(int id);
        void AddStudent(Student student);
        void UpdateStudent(Student student);
        void DeleteStudent(Student student);
        List<Student> GetStudentsByRoomID(int roomId);
        List<Student> GetStudentsWithRoomAttached(string lastName = "");
        void UseVouchers(int studentId, int quantity);
        void AddVouchers(int studentId, int amount);



        // ------- PARTICIPANTS ------- //

        List<Student> GetParticipantsForActivity(int activityId);
        List<Student> GetNonParticipantsForActivity(int activityId);
        void AddParticipant(int activityId, int studentId);
        void RemoveParticipant(int activityId, int studentId);
        ActivityParticipantsViewModel GetActivityParticipants(int activityId, string? message);
    }
}
