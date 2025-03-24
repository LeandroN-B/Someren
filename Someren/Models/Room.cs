using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Someren.Models
{
    // BUILDING ENUM
    public enum BuildingType
    {
        A,
        B
    }

    // Room Type ENUM
    public enum RoomType
    {
        Dormitory,
        Single
    }

    // Class Room
    public class Room
    {
        public int RoomID { get; set; }

        [Required]
        [StringLength(10)]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        public RoomType RoomType { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        public int Floor { get; set; }

        [Required]
        public BuildingType Building { get; set; }

        // Lecturer Foreign key
        public int? LecturerID { get; set; }
        public Lecturer? Lecturer { get; set; }


        public List<Student>? Students { get; set; }

        public Room() { }

        // Constructor with parameters
        public Room(string roomNumber, RoomType roomType, int capacity, int floor, BuildingType building, int? lecturerID = null, List<Student>? students = null)
        {
            RoomNumber = roomNumber;
            RoomType = roomType;
            Capacity = capacity;
            Floor = floor;
            Building = building;
            LecturerID = lecturerID;
            Students = students;
        }

    }
}