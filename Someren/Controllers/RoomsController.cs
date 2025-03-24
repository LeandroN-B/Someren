using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;

namespace Someren.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomRepository _roomRepository;
        private readonly ILecturerRepository _lecturerRepository;
        private readonly IStudentRepository _studentRepository;

        public RoomsController(IRoomRepository roomRepository, ILecturerRepository lecturerRepository, IStudentRepository studentRepository)
        {
            _roomRepository = roomRepository;
            _lecturerRepository = lecturerRepository;
            _studentRepository = studentRepository;
        }

        public IActionResult Index(int capacity = 0)
        {
            List<Room> rooms = _roomRepository.GetAllRooms();
            List<Lecturer> lecturers = _lecturerRepository.GetAllLecturers();
            List<Student> students = _studentRepository.GetAllStudents();

            SetRoomPeople(rooms, lecturers, students);

            if (capacity > 0)
            {
                List<Room> filtered = new List<Room>();
                foreach (Room room in rooms)
                    if (room.Capacity == capacity) filtered.Add(room);
                rooms = filtered;
            }

            return View((rooms, capacity));
        }

        // Private because it's being used only inside the controller
        private void SetRoomPeople(List<Room> rooms, List<Lecturer> lecturers, List<Student> students)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                Room room = rooms[i];

                // Assign lecturer
                foreach (Lecturer lecturer in lecturers)
                {
                    if (lecturer.RoomID == room.RoomID)
                    {
                        room.Lecturer = lecturer;
                        break;
                    }
                }

                // Assign students
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








        [HttpGet]
        public IActionResult Create()
        {
            // Get all lecturers and all rooms
            List<Lecturer> lecturers = _lecturerRepository.GetAllLecturers();
            List<Room> rooms = _roomRepository.GetAllRooms();

            // Return the empty Room model for the view form
            return View(new Room());
        }



        [HttpPost]
        public IActionResult Create(Room room)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(room);
                }

                _roomRepository.AddRoom(room);

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(room);
            }
        }


        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Room? room = _roomRepository.GetRoomByID((int)id);
            return View(room);
        }

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

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Room? room = _roomRepository.GetRoomByID((int)id);

            if (room == null)
            {
                return NotFound();
            }

            if (room.RoomType == RoomType.Single)
            {
                room.Lecturer = _lecturerRepository.GetLecturerByRoomID(room.RoomID);
            }
            else if (room.RoomType == RoomType.Dormitory)
            {
                room.Students = _studentRepository.GetStudentsByRoomID(room.RoomID);
            }

            return View(room);
        }


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

        //
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Room? room = _roomRepository.GetRoomByID((int)id);

            if (room == null)
            {
                return NotFound();
            }

            if (room.RoomType == RoomType.Single)
            {
                room.Lecturer = _lecturerRepository.GetLecturerByRoomID(room.RoomID);
            }
            else if (room.RoomType == RoomType.Dormitory)
            {
                room.Students = _studentRepository.GetStudentsByRoomID(room.RoomID);
            }

            return View(room);
        }


    }
}