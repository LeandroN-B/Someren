using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;

namespace Someren.Controllers
{
    public class LecturersController : Controller
    {
        private readonly ILecturerRepository _lecturerRepository;
        private readonly IRoomRepository _roomRepository;

        public LecturersController(ILecturerRepository lecturerRepository, IRoomRepository roomRepository)
        {
            _lecturerRepository = lecturerRepository;
            _roomRepository = roomRepository;
        }

        public IActionResult Index(string? lastName)
        {
            ViewBag.FilteredLastName = lastName;

            // Ensure lastName is not null when calling the repository
            string safeLastName = lastName ?? "";

            List<Lecturer> lecturers = _lecturerRepository.GetAllLecturers(lastName);

            return View(lecturers);
        }



        [HttpGet]
        public IActionResult Create()
        {
            var availableRooms = _roomRepository.GetAvailableSingleRooms(); // only free single rooms
            ViewData["Rooms"] = availableRooms;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Lecturer lecturer)
        {
            try
            {
                // Check if RoomID is null or 0 (invalid)
                if (lecturer.RoomID == null || lecturer.RoomID == 0)
                {
                    ModelState.AddModelError("", "Please select a room.");
                    // fall through to view return at the end
                }
                else
                {
                    _lecturerRepository.AddLecturer(lecturer);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }

            // If validation failed or an exception happened, repopulate rooms and return the view
            ViewData["Rooms"] = _roomRepository.GetAvailableSingleRooms();
            return View(lecturer);
        }
        
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Lecturer? lecturer = _lecturerRepository.GetLecturerByID((int)id);
            if (lecturer is null)
            {
                return NotFound();
            }
            return View(lecturer);
        }

        [HttpPost]
        public IActionResult Delete(Lecturer lecturer)
        {
            try
            {
                _lecturerRepository.DeleteLecturer(lecturer);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(lecturer);
            }
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            Lecturer? lecturer = _lecturerRepository.GetLecturerByID((int)id);
            if (lecturer == null)
                return NotFound();

            var availableRooms = _roomRepository.GetAvailableSingleRooms(lecturer.RoomID); // also include the current room
            ViewBag.AvailableRooms = availableRooms;

            return View(lecturer);
        }
        [HttpPost]
        public IActionResult Edit(Lecturer lecturer)
        {
            try
            {
                _lecturerRepository.UpdateLecturer(lecturer);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                // Fallback to repopulate dropdown if something breaks
                var availableRooms = _roomRepository.GetAvailableSingleRooms(lecturer.RoomID);
                ViewBag.AvailableRooms = availableRooms;

                return View(lecturer);
            }
        }



    }
}
