using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

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

            List<Student> students;

            if (!string.IsNullOrEmpty(lastName))
            {
                students = _studentRepository.GetStudentsByLastName(lastName)
                                             .OrderBy(s => s.LastName)
                                             .ToList();
            }
            else
            {
                students = _studentRepository.GetAllStudents()
                                             .OrderBy(s => s.LastName)
                                             .ToList();
            }

            return View(students);
        }

        [HttpGet]
        public IActionResult Create()
        {
            List<Room> rooms = _roomRepository.GetAllRooms();
            List<Room> availableDorms = new List<Room>();

            foreach (Room room in rooms)
            {
                if (room.RoomType == RoomType.Dormitory)
                {
                    var studentsInRoom = _studentRepository.GetStudentsByRoomID(room.RoomID);
                    if (studentsInRoom.Count < room.Capacity)
                    {
                        availableDorms.Add(room);
                    }
                }
            }

            ViewData["Rooms"] = availableDorms;
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
                    ViewData["Rooms"] = GetAvailableDorms();
                    return View(student);
                }

                if (!CanAddStudentToRoom(student.RoomID.Value))
                {
                    ModelState.AddModelError("", "Selected room is already full.");
                    ViewData["Rooms"] = GetAvailableDorms();
                    return View(student);
                }

                _studentRepository.AddStudent(student);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                ViewData["Rooms"] = GetAvailableDorms();
                return View(student);
            }
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            Student? student = _studentRepository.GetStudentByID((int)id);
            if (student == null) return NotFound();

            List<Room> allRooms = _roomRepository.GetAllRooms();
            List<Room> availableRooms = new List<Room>();

            foreach (Room room in allRooms)
            {
                if (room.RoomType != RoomType.Dormitory) continue;

                var studentsInRoom = _studentRepository.GetStudentsByRoomID(room.RoomID);
                bool isCurrentRoom = room.RoomID == student.RoomID;
                bool hasSpace = studentsInRoom.Count < room.Capacity;

                if (hasSpace || isCurrentRoom)
                {
                    availableRooms.Add(room);
                }
            }

            ViewBag.AvailableRooms = availableRooms;
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
                    ViewBag.AvailableRooms = GetAvailableDormsForEdit(student);
                    return View(student);
                }

                if (!CanAssignRoomToStudent(student.StudentID, student.RoomID.Value))
                {
                    ModelState.AddModelError("", "Selected room is already full.");
                    ViewBag.AvailableRooms = GetAvailableDormsForEdit(student);
                    return View(student);
                }

                _studentRepository.UpdateStudent(student);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                ViewBag.AvailableRooms = GetAvailableDormsForEdit(student);
                return View(student);
            }
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            Student? student = _studentRepository.GetStudentByID((int)id);
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
            catch (Exception)
            {
                return View(student);
            }
        }

        // === Helpers ===

        private bool CanAddStudentToRoom(int roomId)
        {
            var room = _roomRepository.GetRoomByID(roomId);
            if (room == null || room.RoomType != RoomType.Dormitory)
                return false;

            var students = _studentRepository.GetStudentsByRoomID(roomId);
            return students.Count < room.Capacity;
        }

        private bool CanAssignRoomToStudent(int studentId, int roomId)
        {
            var room = _roomRepository.GetRoomByID(roomId);
            if (room == null || room.RoomType != RoomType.Dormitory)
                return false;

            var students = _studentRepository.GetStudentsByRoomID(roomId);
            return students.Count < room.Capacity || students.Any(s => s.StudentID == studentId);
        }

        private List<Room> GetAvailableDorms()
        {
            return _roomRepository.GetAllRooms()
                .Where(r => r.RoomType == RoomType.Dormitory &&
                            _studentRepository.GetStudentsByRoomID(r.RoomID).Count < r.Capacity)
                .ToList();
        }

        private List<Room> GetAvailableDormsForEdit(Student student)
        {
            var rooms = GetAvailableDorms();
            var currentRoom = _roomRepository.GetRoomByID(student.RoomID ?? 0);
            if (currentRoom != null && currentRoom.RoomType == RoomType.Dormitory)
            {
                if (!rooms.Any(r => r.RoomID == currentRoom.RoomID))
                {
                    rooms.Add(currentRoom);
                }
            }

            return rooms.OrderBy(r => r.RoomNumber).ToList();
        }
    }
}
