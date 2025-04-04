namespace Someren.Models
{
    public class Drink
    {
        public int DrinkId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public decimal VAT { get; set; }
        public int DefaultStock { get; set; }
        public bool IsAlcoholic { get; set; }
        public int TotalOrderedByStudents { get; set; }
        public int TotalOrderedByLecturers { get; set; }
    }
}
