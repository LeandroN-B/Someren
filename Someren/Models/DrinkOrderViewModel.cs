using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Someren.Models
{
    public class DrinkOrderViewModel
    {
        [ValidateNever]
        public List<Student> Students { get; set; } = new();

        [ValidateNever]
        public List<Lecturer> Lecturers { get; set; } = new();

        [ValidateNever]
        public List<Drink> Drinks { get; set; } = new();

        public int SelectedStudentId { get; set; }
        public int SelectedLecturerId { get; set; }
        public int SelectedDrinkId { get; set; }
        public int Quantity { get; set; }
        public string UserType { get; set; } = string.Empty;
        public int VouchersRemaining { get; set; }

        public List<SelectListItem> StudentSelectList { get; set; } = new();
        public List<SelectListItem> LecturerSelectList { get; set; } = new();
        public List<SelectListItem> DrinkSelectList { get; set; } = new();
    }
}
