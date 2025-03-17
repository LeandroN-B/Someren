using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Someren.Models
{
    public class Student
    {
        private string? telephone;
        private string? className;

        public Student(int studentID, string? firstName, string? lastName, string? telephone, string? className)
        {
            StudentID = studentID;
            FirstName = firstName;
            LastName = lastName;
            this.telephone = telephone;
            this.className = className;
        }

        public Student(int studentID, string? firstName, string? lastName, string? phoneNumber, DateTime dateOfBirth)
        {
            StudentID = studentID;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
        }

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

        public string FullName => $"{FirstName} {LastName}";

        public int Age
        {
            get
            {
                int age = DateTime.Now.Year - DateOfBirth.Year;
                if (DateTime.Now < DateOfBirth.AddYears(age)) age--;
                return age;
            }
        }

        public object Class { get; internal set; }
        public object Telephone { get; internal set; }

        internal static void Add(Student student)
        {
            throw new NotImplementedException();
        }
    }
}
