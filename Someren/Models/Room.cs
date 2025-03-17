using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        public Room() { }

        // Constructor with parameters
        public Room(string roomNumber, RoomType roomType, int capacity, int floor, BuildingType building)
        {
            RoomNumber = roomNumber;
            RoomType = roomType;
            Capacity = capacity;
            Floor = floor;
            Building = building;
        }
    }
}

/* EXTRA INFORMATION

[REQUIRED] means Mandatory NOT NULL
[KEY] means it is PRIMARY KEY
[] The database will automatically generate a unique number/value/ID for this column when a new room is added.

(My main question is does it ever reset and goes back to starting at 1?)
 */