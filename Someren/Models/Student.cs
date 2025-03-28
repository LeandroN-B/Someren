using System;

namespace Someren.Models
{
    public class Student
    {
        public int StudentID { get; set; }

        public string StudentNumber { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string ClassName { get; set; } = string.Empty;

        public int? RoomID { get; set; }

        public Room? Room { get; set; }



        public Student() { }

        public Student(int studentID, string studentNumber, string firstName, string lastName, string phoneNumber, string className, int? roomID)
        {
            StudentID = studentID;
            StudentNumber = studentNumber;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            ClassName = className;
            RoomID = roomID;
        }
    }
}
