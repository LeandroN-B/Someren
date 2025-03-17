using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;

namespace Someren.Controllers
{
    public class LecturersController : Controller
    {
        private readonly ILecturerRepository _lecturerRepository;
        private readonly IRoomRepository _roomRepository;

        //the dependencies should be injected through a ctor
        public LecturersController(ILecturerRepository lecturerRepository, IRoomRepository roomRepository)
        {
            _lecturerRepository = lecturerRepository;
            _roomRepository = roomRepository;
        }

        public IActionResult Index()
        {
            List<Lecturer> lecturers = _lecturerRepository.GetAllLecturers()
                                          .OrderBy(l => l.LastName) // Sort by Last Name (A-Z)
                                          .ToList();
            return View(lecturers);
        }


            [HttpGet]
        public IActionResult Create()
        {
            try
            {
                // Fetch all rooms using _roomRepository
                var rooms = _roomRepository.GetAllRooms();

                // Check if rooms are null or empty for debugging
                if (rooms == null || rooms.Count == 0)
                {
                    throw new InvalidOperationException("No rooms available.");
                }

                // Pass rooms to the view
                ViewData["Rooms"] = rooms;

                return View();
            }
            catch (Exception ex)
            {
                // Log the error (you can use a logger here)
                ModelState.AddModelError("", $"Error fetching rooms: {ex.Message}");
                return View();
            }
        }


        [HttpPost]
        public IActionResult Create(Lecturer lecturer)
        {
            try
            {
                if (lecturer.RoomID == 0)
                {
                    // Handle error: No valid room selected
                    ModelState.AddModelError("", "Please select a room.");
                    return View(lecturer);
                }

                _lecturerRepository.AddLecturer(lecturer);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle exception
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(lecturer);
            }
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
            if (id is null)
            {
                return NotFound();
            }

            Lecturer? lecturer = _lecturerRepository.GetLecturerByID((int)id);
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
            catch (Exception)
            {
                return View(lecturer);
            }
        }
        
    }
}
