using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Someren.Models
{
    public class Student
    {
        private string? telephone;
        private string? className;
     
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
        public int RoomID { get; set; }// I am not going to use for the first assignement

        public string FullName => $"{FirstName} {LastName}";

       
        public string ClassName { get; set; }


        public Student(int studentID, string firstName, string lastName, string phoneNumber, string className)
        {
            StudentID = studentID;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            ClassName = className;
        }

    }
}
