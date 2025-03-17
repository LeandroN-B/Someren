namespace Someren.Models
{
    public class Lecturer
    {
        public int LecturerID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int RoomID { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CheckInDate { get; set; } = DateTime.Now;
        public DateTime CheckOutDate { get; set; } = DateTime.Now.AddDays(7);

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
