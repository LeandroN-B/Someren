using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;

namespace Someren.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IRoomRepository _roomRepository;

        public StudentsController(IStudentRepository studentRepository, IRoomRepository roomRepository)
        {
            _studentRepository = studentRepository;
            _roomRepository = roomRepository;
        }

        public IActionResult Index(string? lastName)
        {
            ViewBag.FilteredLastName = lastName;
            string safeLastName = lastName ?? "";
            List<Student> students = _studentRepository.GetStudentsWithRoomAttached(safeLastName);

            return View(students);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Rooms"] = GetAvailableDormRooms();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Student student)
        {
            try
            {
                if (student.RoomID == null || student.RoomID == 0)
                {
                    ModelState.AddModelError("", "Please select a room.");
                }
                else if (!RoomHasSpace(student.RoomID.Value))
                {
                    ModelState.AddModelError("", "Selected room is already full.");
                }
                else
                {
                    _studentRepository.AddStudent(student);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
            }

            ViewData["Rooms"] = GetAvailableDormRooms();
            return View(student);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            Student? student = _studentRepository.GetStudentByID(id.Value);
            if (student == null) return NotFound();

            ViewBag.AvailableRooms = GetAvailableDormRoomsForEdit(student);
            return View(student);
        }

        [HttpPost]
        public IActionResult Edit(Student student)
        {
            try
            {
                if (student.RoomID == null || student.RoomID == 0)
                {
                    ModelState.AddModelError("", "Please select a room.");
                }
                else if (!RoomCanBeAssigned(student.StudentID, student.RoomID.Value))
                {
                    ModelState.AddModelError("", "Selected room is already full.");
                }
                else
                {
                    _studentRepository.UpdateStudent(student);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
            }

            ViewBag.AvailableRooms = GetAvailableDormRoomsForEdit(student);
            return View(student);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            Student? student = _studentRepository.GetStudentByID(id.Value);
            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost]
        public IActionResult Delete(Student student)
        {
            try
            {
                _studentRepository.DeleteStudent(student);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(student);
            }
        }

        // === Helper Methods ===

        private List<Room> GetAvailableDormRooms()
        {
            List<Room> availableDorms = new List<Room>();
            List<Room> allRooms = _roomRepository.GetAllRooms();

            foreach (Room room in allRooms)
            {
                if (room.RoomType == RoomType.Dormitory)
                {
                    List<Student> studentsInRoom = _studentRepository.GetStudentsByRoomID(room.RoomID);
                    if (studentsInRoom.Count < room.Capacity)
                    {
                        availableDorms.Add(room);
                    }
                }
            }

            return availableDorms;
        }

        private List<Room> GetAvailableDormRoomsForEdit(Student student)
        {
            List<Room> availableRooms = GetAvailableDormRooms();
            Room? currentRoom = _roomRepository.GetRoomByID(student.RoomID ?? 0);

            bool alreadyIncluded = false;
            foreach (Room room in availableRooms)
            {
                if (room.RoomID == currentRoom?.RoomID)
                {
                    alreadyIncluded = true;
                    break;
                }
            }

            if (!alreadyIncluded && currentRoom != null && currentRoom.RoomType == RoomType.Dormitory)
            {
                availableRooms.Add(currentRoom);
            }

            return availableRooms;
        }

        private bool RoomHasSpace(int roomId)
        {
            Room? room = _roomRepository.GetRoomByID(roomId);
            if (room == null || room.RoomType != RoomType.Dormitory)
                return false;

            List<Student> students = _studentRepository.GetStudentsByRoomID(roomId);
            return students.Count < room.Capacity;
        }

        private bool RoomCanBeAssigned(int studentId, int roomId)
        {
            Room? room = _roomRepository.GetRoomByID(roomId);
            if (room == null || room.RoomType != RoomType.Dormitory)
                return false;

            List<Student> students = _studentRepository.GetStudentsByRoomID(roomId);

            foreach (Student s in students)
            {
                if (s.StudentID == studentId)
                    return true; // allow current student to stay in their room
            }

            return students.Count < room.Capacity;
        }
    }
}
