using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Someren.Models
{
    // Building ENUM (for reference; note that the Room model uses string for Building)
    public enum BuildingType
    {
        Single,
        Dormitory
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
        public char RoomType { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        public int Floor { get; set; }

        [Required]
        [Column(TypeName = "varchar(10)")]
        public string Building { get; set; } = "Single";

        public Room() { }

        // Constructor with parameters
        public Room(string roomNumber, char roomType, int capacity, int floor, string building)
        {
            RoomNumber = roomNumber;
            RoomType = roomType;
            Capacity = capacity;
            Floor = floor;
            Building = building;
        }
    }
}

