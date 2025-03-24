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

            List<Lecturer> lecturers;

            if (!string.IsNullOrEmpty(lastName))
            {
                // Filtered list (and sorted by last name)
                lecturers = _lecturerRepository.GetLecturersByLastName(lastName)
                                               .OrderBy(l => l.LastName)
                                               .ToList();
            }
            else
            {
                // Full list sorted by last name
                lecturers = _lecturerRepository.GetAllLecturers()
                                               .OrderBy(l => l.LastName)
                                               .ToList();
            }

            return View(lecturers);
        }



        [HttpGet]
        public IActionResult Create()
        {
            // Get all rooms from the database
            List<Room> rooms = _roomRepository.GetAllRooms();

            // Filter only available single rooms
            List<Room> availableRooms = new List<Room>();

            foreach (Room room in rooms)
            {
                if (room.RoomType == RoomType.Single && _lecturerRepository.IsRoomFreeForAddLecturer(room.RoomID))
                {
                    availableRooms.Add(room);
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
                    ModelState.AddModelError("", "Please select a room.");
                    return View(lecturer);
                }

                if (!_lecturerRepository.IsRoomFreeForAddLecturer(lecturer.RoomID))
                {
                    ModelState.AddModelError("", "This room is already assigned to a lecturer.");
                    return View(lecturer);
                }

                _lecturerRepository.AddLecturer(lecturer);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
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
            if (id == null)
                return NotFound();

            Lecturer? lecturer = _lecturerRepository.GetLecturerByID((int)id);
            if (lecturer == null)
                return NotFound();

            // A list of available rooms but only Single type and unoccupied or current room)
            List<Room> allRooms = _roomRepository.GetAllRooms();
            List<Room> availableRooms = new List<Room>();

            foreach (Room room in allRooms)
            {
                bool isSingleRoom = room.RoomType == RoomType.Single;
                bool isUnoccupied = _lecturerRepository.GetLecturerByRoomID(room.RoomID) == null;
                bool isCurrentRoom = room.RoomID == lecturer.RoomID;

                if (isSingleRoom && (isUnoccupied || isCurrentRoom))
                {
                    availableRooms.Add(room);
                }
            }

            ViewBag.AvailableRooms = availableRooms;

            return View(lecturer);
        }

        [HttpPost]
        public IActionResult Edit(Lecturer lecturer)
        {
            try
            {
                // Check if the selected room is available
                if (!_lecturerRepository.IsRoomFreeForEditLecturer(lecturer.LecturerID, lecturer.RoomID))
                {
                    ModelState.AddModelError("", "Selected room is not available or invalid.");

                    // Build list of available rooms manually
                    var allRooms = _roomRepository.GetAllRooms();
                    var availableRooms = new List<Room>();

                    foreach (Room room in allRooms)
                    {
                        bool isSingleRoom = room.RoomType == RoomType.Single;
                        bool isUnoccupied = _lecturerRepository.GetLecturerByRoomID(room.RoomID) == null;
                        bool isCurrentRoom = room.RoomID == lecturer.RoomID;

                        if (isSingleRoom && (isUnoccupied || isCurrentRoom))
                        {
                            availableRooms.Add(room);
                        }
                    }

                    ViewBag.AvailableRooms = availableRooms;
                    return View(lecturer);
                }

                // If room is valid, update the lecturer
                _lecturerRepository.UpdateLecturer(lecturer);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                // Build available rooms again in case of error
                var allRooms = _roomRepository.GetAllRooms();
                var availableRooms = new List<Room>();

                foreach (Room room in allRooms)
                {
                    bool isSingleRoom = room.RoomType == RoomType.Single;
                    bool isUnoccupied = _lecturerRepository.GetLecturerByRoomID(room.RoomID) == null;
                    bool isCurrentRoom = room.RoomID == lecturer.RoomID;

                    if (isSingleRoom && (isUnoccupied || isCurrentRoom))
                    {
                        availableRooms.Add(room);
                    }
                }

                ViewBag.AvailableRooms = availableRooms;
                return View(lecturer);
            }
        }
    }
}