namespace Someren.Models
{
    public class DrinkOrder
    {
        public int DrinkOrderId { get; set; }
        public int StudentId { get; set; }
        public int DrinkId { get; set; }
        public int Quantity { get; set; }

        public Student? Student { get; set; }
        public Drink? Drink { get; set; }
    }
}
