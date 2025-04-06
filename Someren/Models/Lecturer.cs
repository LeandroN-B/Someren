using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Someren.Models
{
    public class Lecturer
    {
        //primary key (PK)
        public int LecturerID { get; set; }
        //required attributes in the assignments. 'string.Empty;' to avoid null references issues
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        //foreigner key (FK), it's null in case the Lecturer has no assigned room yet
        public int? RoomID { get; set; }
        
        //attributes I added from my ERD that I didn't use yet
        public DateTime? LecInDate { get; set; } = DateTime.Now;
        public DateTime? LecOutDate { get; set; } = DateTime.Now.AddDays(7);

        public string FullName => $"{FirstName} {LastName}";

        public int Age//calculated property
        {
            get
            {
                int age = DateTime.Now.Year - DateOfBirth.Year;
                if (DateTime.Now < DateOfBirth.AddYears(age)) age--;
                return age;
            }
        }

        //ctor parameterless
        public Lecturer() { }

        public Lecturer(int lecturerID, string firstName, string lastName, string phoneNumber, DateTime dateOfBirth, int roomID)
        {
            LecturerID = lecturerID;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
            RoomID = roomID;
        }
    }
}
