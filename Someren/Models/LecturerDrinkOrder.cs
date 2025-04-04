namespace Someren.Models
{
    public class LecturerDrinkOrder
    {
        public int OrderId { get; set; }
        public int LecturerId { get; set; }
        public int DrinkId { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
