using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Someren.Controllers
{
    public class DrinkController : Controller
    {
        private readonly IDrinkRepository _drinkRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ILecturerRepository _lecturerRepository;
        private readonly IDrinkOrderRepository _drinkOrderRepository;
        private readonly ILecturerDrinkOrderRepository _lecturerDrinkOrderRepository;

        public DrinkController(
            IDrinkRepository drinkRepository,
            IStudentRepository studentRepository,
            IDrinkOrderRepository drinkOrderRepository,
            ILecturerRepository lecturerRepository,
            ILecturerDrinkOrderRepository lecturerDrinkOrderRepository)
        {
            _drinkRepository = drinkRepository;
            _studentRepository = studentRepository;
            _drinkOrderRepository = drinkOrderRepository;
            _lecturerRepository = lecturerRepository;
            _lecturerDrinkOrderRepository = lecturerDrinkOrderRepository;
        }

        public IActionResult Index()
        {
            var drinks = _drinkRepository.GetAllDrinks();
            return View(drinks);
        }

        public IActionResult Order(int? selectedStudentId = null, int? selectedLecturerId = null, int? selectedDrinkId = null, int? quantity = null, string userType = "Student")
        {
            var students = _studentRepository.GetAllStudents();
            var lecturers = _lecturerRepository.GetAllLecturers();
            var drinks = _drinkRepository.GetAllDrinks();

            int maxQuantity = 0;

            if (selectedDrinkId.HasValue)
            {
                var drink = drinks.FirstOrDefault(d => d.DrinkId == selectedDrinkId.Value);
                if (drink != null)
                {
                    if (userType == "Student" && selectedStudentId.HasValue)
                    {
                        var student = students.FirstOrDefault(s => s.StudentID == selectedStudentId);
                        if (student != null)
                        {
                            maxQuantity = Math.Min(student.Vouchers, drink.Stock);
                        }
                    }
                    else
                    {
                        maxQuantity = drink.Stock;
                    }
                }
            }

            var viewModel = new DrinkOrderViewModel
            {
                Students = students,
                Lecturers = lecturers,
                Drinks = drinks,
                UserType = userType,
                SelectedStudentId = selectedStudentId ?? 0,
                SelectedLecturerId = selectedLecturerId ?? 0,
                SelectedDrinkId = selectedDrinkId ?? 0,
                Quantity = maxQuantity,
                StudentSelectList = students.Select(s => new SelectListItem
                {
                    Value = s.StudentID.ToString(),
                    Text = $"{s.FirstName} {s.LastName}",
                    Selected = s.StudentID == selectedStudentId
                }).ToList(),
                LecturerSelectList = lecturers.Select(l => new SelectListItem
                {
                    Value = l.LecturerID.ToString(),
                    Text = $"{l.FirstName} {l.LastName}",
                    Selected = l.LecturerID == selectedLecturerId
                }).ToList(),
                DrinkSelectList = drinks.Select(d => new SelectListItem
                {
                    Value = d.DrinkId.ToString(),
                    Text = $"{d.Name} ({d.Stock} in stock)",
                    Selected = d.DrinkId == selectedDrinkId
                }).ToList(),
            };

            if (userType == "Student" && selectedStudentId.HasValue)
            {
                var student = students.FirstOrDefault(s => s.StudentID == selectedStudentId);
                if (student != null)
                {
                    viewModel.VouchersRemaining = student.Vouchers;

                    if (selectedDrinkId.HasValue)
                    {
                        var drink = drinks.FirstOrDefault(d => d.DrinkId == selectedDrinkId.Value);
                        if (drink != null)
                        {
                            viewModel.Quantity = Math.Min(student.Vouchers, drink.Stock);
                        }
                    }
                }
            }
            else if (selectedDrinkId.HasValue)
            {
                var drink = drinks.FirstOrDefault(d => d.DrinkId == selectedDrinkId.Value);
                if (drink != null)
                {
                    viewModel.Quantity = drink.Stock;
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Order(DrinkOrderViewModel model)
        {
            var drinks = _drinkRepository.GetAllDrinks();
            var selectedDrink = drinks.FirstOrDefault(d => d.DrinkId == model.SelectedDrinkId);

            if (selectedDrink == null)
            {
                TempData["ErrorMessage"] = "🚫 Selected drink not found.";
                TempData["FormSubmitted"] = true;
                return RedirectToOrder(model);
            }

            if (model.UserType == "Student" && selectedDrink.IsAlcoholic)
            {
                TempData["ErrorMessage"] = "🚫 Students cannot order alcoholic drinks!";
                TempData["FormSubmitted"] = true;
                return RedirectToOrder(model);
            }

            if (selectedDrink.Stock == 0)
            {
                TempData["ErrorMessage"] = "🚫 Drink is out of stock!";
                TempData["FormSubmitted"] = true;
                return RedirectToOrder(model);
            }

            if (model.Quantity <= 0)
            {
                TempData["ErrorMessage"] = "🚫 Quantity must be at least 1!";
                TempData["FormSubmitted"] = true;
                return RedirectToOrder(model);
            }

            return RedirectToAction("ConfirmOrder", new
            {
                selectedStudentId = model.SelectedStudentId,
                selectedLecturerId = model.SelectedLecturerId,
                selectedDrinkId = model.SelectedDrinkId,
                quantity = model.Quantity,
                userType = model.UserType
            });
        }

        [HttpGet]
        public IActionResult ConfirmOrder(int selectedStudentId, int selectedLecturerId, int selectedDrinkId, int quantity, string userType)
        {
            var viewModel = new DrinkOrderViewModel
            {
                Students = _studentRepository.GetAllStudents(),
                Lecturers = _lecturerRepository.GetAllLecturers(),
                Drinks = _drinkRepository.GetAllDrinks(),
                SelectedStudentId = selectedStudentId,
                SelectedLecturerId = selectedLecturerId,
                SelectedDrinkId = selectedDrinkId,
                Quantity = quantity,
                UserType = userType
            };

            if (userType == "Student")
            {
                var student = viewModel.Students.FirstOrDefault(s => s.StudentID == selectedStudentId);
                if (student != null)
                {
                    viewModel.VouchersRemaining = student.Vouchers;
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ConfirmOrder(DrinkOrderViewModel model)
        {
            var selectedDrink = _drinkRepository.GetAllDrinks().FirstOrDefault(d => d.DrinkId == model.SelectedDrinkId);

            if (model.UserType == "Student")
            {
                var student = _studentRepository.GetAllStudents().FirstOrDefault(s => s.StudentID == model.SelectedStudentId);

                if (student != null && selectedDrink != null)
                {
                    _studentRepository.UseVouchers(student.StudentID, model.Quantity);

                    var order = new DrinkOrder
                    {
                        StudentId = model.SelectedStudentId,
                        DrinkId = model.SelectedDrinkId,
                        Quantity = model.Quantity
                    };

                    _drinkOrderRepository.AddDrinkOrder(order);

                    TempData["SuccessMessage"] = $"✅ {student.FirstName} {student.LastName} ordered {model.Quantity}x {selectedDrink.Name}!";
                }
            }
            else if (model.UserType == "Lecturer")
            {
                var lecturer = _lecturerRepository.GetAllLecturers().FirstOrDefault(l => l.LecturerID == model.SelectedLecturerId);

                if (lecturer != null && selectedDrink != null)
                {
                    var lecturerOrder = new LecturerDrinkOrder
                    {
                        LecturerId = model.SelectedLecturerId,
                        DrinkId = model.SelectedDrinkId,
                        Quantity = model.Quantity
                    };

                    _drinkRepository.IncrementDrinkOrderCount(model.SelectedDrinkId, model.UserType, model.Quantity);
                    _lecturerDrinkOrderRepository.AddLecturerDrinkOrder(lecturerOrder);
                    _drinkRepository.ReduceStock(model.SelectedDrinkId, model.Quantity);

                    TempData["SuccessMessage"] = $"✅ {lecturer.FirstName} {lecturer.LastName} ordered {model.Quantity}x {selectedDrink.Name}!";
                }
            }

            return RedirectToAction("Order", new
            {
                selectedStudentId = model.SelectedStudentId,
                selectedLecturerId = model.SelectedLecturerId,
                selectedDrinkId = model.SelectedDrinkId,
                userType = model.UserType
            });
        }

        [HttpPost]
        public IActionResult RefillVouchers(int studentId, int amount)
        {
            if (amount > 3)
            {
                amount = 3;
            }

            _studentRepository.AddVouchers(studentId, amount);

            var student = _studentRepository.GetAllStudents().FirstOrDefault(s => s.StudentID == studentId);
            var studentName = student != null ? $"{student.FirstName} {student.LastName}" : "Unknown Student";

            TempData["SuccessMessage"] = $"✅ {amount} vouchers added for {studentName}! You can now place your order.";

            return RedirectToAction("Order", new { selectedStudentId = studentId, userType = "Student" });
        }

        public IActionResult ResetStock()
        {
            _drinkRepository.ResetAllStocks();
            TempData["SuccessMessage"] = "🔁 All drink stocks have been reset!";
            return RedirectToAction("Index");
        }

        private IActionResult RedirectToOrder(DrinkOrderViewModel model)
        {
            return RedirectToAction("Order", new
            {
                selectedStudentId = model.SelectedStudentId,
                selectedLecturerId = model.SelectedLecturerId,
                selectedDrinkId = model.SelectedDrinkId,
                userType = model.UserType
            });
        }
    }
}
