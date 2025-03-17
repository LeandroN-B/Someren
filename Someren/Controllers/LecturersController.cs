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
            List<Lecturer> lecturers = _lecturerRepository.GetAllLecturers();
            return View(lecturers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Get all rooms from the database
            List<Room> rooms = _roomRepository.GetAllRooms();

            // Create a list to store available rooms
            List<Room> availableRooms = new List<Room>();

            // Loop through each room and check if it's available
            foreach (Room room in rooms)
            {
                // Only check Single rooms
                if (room.RoomType == RoomType.Single)
                {
                    // Get lecturers in the room
                    List<Lecturer> lecturersInRoom = _lecturerRepository.GetLecturersInRoom(room.RoomID);

                    // If no lecturer is assigned, add room to availableRooms
                    if (lecturersInRoom.Count == 0)
                    {
                        availableRooms.Add(room);
                    }
                }
            }

            // Pass the available rooms to the view
            ViewData["Rooms"] = availableRooms;

            return View();
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
