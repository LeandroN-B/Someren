using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;

namespace Someren.Controllers
{
    public class RoomsController : Controller
    {
        // Repositories to access room, lecturer, and student data
        private readonly IRoomRepository _roomRepository;
        private readonly ILecturerRepository _lecturerRepository;
        private readonly IStudentRepository _studentRepository;

        // Constructor to inject the repositories
        public RoomsController(IRoomRepository roomRepository, ILecturerRepository lecturerRepository, IStudentRepository studentRepository)
        {
            _roomRepository = roomRepository;
            _lecturerRepository = lecturerRepository;
            _studentRepository = studentRepository;
        }

        // Displays the list of rooms (with optional capacity filtering)
        public IActionResult Index(int capacity = 0)
        {
            List<Room> rooms = _roomRepository.GetAllRooms();
            List<Lecturer> lecturers = _lecturerRepository.GetAllLecturers();
            List<Student> students = _studentRepository.GetAllStudents();

            // Attach lecturers and students to their respective rooms
            SetRoomPeople(rooms, lecturers, students);

            // If a specific capacity is selected, filter the list
            if (capacity > 0)
            {
                List<Room> filtered = new List<Room>();
                foreach (Room room in rooms)
                    if (room.Capacity == capacity) filtered.Add(room);
                rooms = filtered;
            }

            return View((rooms, capacity));
        }

        // Assigns lecturers and students to rooms for displaying
        private void SetRoomPeople(List<Room> rooms, List<Lecturer> lecturers, List<Student> students)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                Room room = rooms[i];

                // Assign lecturer to room (if matched by RoomID)
                foreach (Lecturer lecturer in lecturers)
                {
                    if (lecturer.RoomID == room.RoomID)
                    {
                        room.Lecturer = lecturer;
                        break;
                    }
                }

                // Assign students to the room (if matched by RoomID)
                room.Students = new List<Student>();
                foreach (Student student in students)
                {
                    if (student.RoomID == room.RoomID)
                    {
                        room.Students.Add(student);
                    }
                }
            }
        }

        // Shows the Create Room form
        [HttpGet]
        public IActionResult Create()
        {
            // Returns an empty Room object to fill in the form
            return View(new Room());
        }

        // Handles submission of the Create Room form
        [HttpPost]
        public IActionResult Create(Room room)
        {
            // validation for required fields
            if (!ModelState.IsValid) return View(room);

            // Prevent adding duplicate room numbers
            if (RoomNumberExists(room.RoomNumber))
            {
                ModelState.AddModelError("RoomNumber", "This room number already exists.");
                return View(room);
            }

            try
            {
                _roomRepository.AddRoom(room);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(room);
            }
        }

        // Checks if a room with the same number already exists
        private bool RoomNumberExists(string roomNumber)
        {
            List<Room> allRooms = _roomRepository.GetAllRooms();

            foreach (Room room in allRooms)
            {
                if (room.RoomNumber == roomNumber)
                {
                    return true;
                }
            }

            return false;
        }

        // Shows the delete confirmation page
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id is null) return NotFound();

            Room? room = _roomRepository.GetRoomByID((int)id);
            return View(room);
        }

        // Handles the deletion after confirmation
        [HttpPost]
        public IActionResult Delete(Room room)
        {
            try
            {
                _roomRepository.DeleteRoom(room);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(room);
            }
        }

        // Displays the Edit Room form with current data
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id is null) return NotFound();

            Room? room = _roomRepository.GetRoomByID((int)id);
            if (room == null) return NotFound();

            // Load current person data
            if (room.RoomType == RoomType.Single)
                room.Lecturer = _lecturerRepository.GetLecturerByRoomID(room.RoomID);
            else if (room.RoomType == RoomType.Dormitory)
                room.Students = _studentRepository.GetStudentsByRoomID(room.RoomID);

            return View(room);
        }

        // Saves the updated room data
        [HttpPost]
        public IActionResult Edit(Room room)
        {
            try
            {
                _roomRepository.UpdateRoom(room);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(room);
            }
        }

        // Shows room details (read-only)
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id is null) return NotFound();

            Room? room = _roomRepository.GetRoomByID((int)id);
            if (room == null) return NotFound();

            // Attach occupant info for display
            if (room.RoomType == RoomType.Single)
                room.Lecturer = _lecturerRepository.GetLecturerByRoomID(room.RoomID);
            else if (room.RoomType == RoomType.Dormitory)
                room.Students = _studentRepository.GetStudentsByRoomID(room.RoomID);

            return View(room);
        }
    }
}
