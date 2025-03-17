namespace Someren.Models
{
    public class Lecturer
    {       
        public int LecturerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int RoomID { get; set; }// I am not going to use for the first assignement
        public DateTime DateOfBirth { get; set; }// I am not going to use for the first assignement
        public DateTime CheckInDate { get; set; }// I am not going to use for the first assignement
        public DateTime CheckOutDate { get; set; }// I am not going to use for the first assignement
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
        public Lecturer(int lecturerID, string firstName, string lastName, string phoneNumber, DateTime dateOfBirth)
        {
            LecturerID = lecturerID;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
        }
    }
}
