using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Someren.Models
{
    public class Student
    {
        private string? telephone;
        private string? className;

        public int StudentID { get; set; }

        public string StudentNumber { get; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
        public int? RoomID { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string ClassName { get; set; }


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
