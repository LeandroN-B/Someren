using System;
using System.ComponentModel.DataAnnotations;

namespace Someren.Models
{
    public class Student
    {
        // Public properties with meaningful names
        public int StudentID { get; set; }

        [Required]
        public string StudentNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "First name can't be longer than 100 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "Last name can't be longer than 100 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(15, ErrorMessage = "Phone number can't be longer than 15 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Class name can't be longer than 50 characters.")]
        public string ClassName { get; set; } = string.Empty;

        public string? Telephone { get; set; }

        public int? RoomID { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        // Parameterless constructor for model binding
        public Student() { }

        // Optional constructor for manual use
        public Student(int studentID, string studentNumber, string firstName, string lastName, string phoneNumber, string className, int? roomID, string? telephone = null)
        {
            StudentID = studentID;
            StudentNumber = studentNumber;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            ClassName = className;
            RoomID = roomID;
            Telephone = telephone;
        }
    }
}
