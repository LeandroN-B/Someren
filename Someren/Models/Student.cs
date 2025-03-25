using System;
using System.ComponentModel.DataAnnotations;

namespace Someren.Models
{
    public class Student
    {
        // Private fields to encapsulate optional properties
        private string? _telephone;
        private string? _className;

        // Public properties with meaningful names
        public int StudentID { get; set; }

        public string StudentNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "First name can't be longer than 100 characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Last name can't be longer than 100 characters.")]
        public string LastName { get; set; }

        [Required]
        [Phone]
        [StringLength(15, ErrorMessage = "Phone number can't be longer than 15 characters.")]
        public string PhoneNumber { get; set; }

        public int? RoomID { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [StringLength(50, ErrorMessage = "Class name can't be longer than 50 characters.")]
        public string ClassName
        {
            get => _className;
            set => _className = value;
        }

        public string Telephone
        {
            get => _telephone;
            set => _telephone = value;
        }

        public Student() { }

        // Constructor to initialize required fields
        public Student(int studentID, string studentNumber, string firstName, string lastName, string phoneNumber, string className, int roomID, string? telephone = null)
        {
            StudentID = studentID;
            StudentNumber = studentNumber;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            ClassName = className;
            RoomID = roomID;
            Telephone = telephone; // Optional telephone number
        }
    }
}
