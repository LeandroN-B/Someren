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
            // Get all rooms, lecturers, and students
            List<Room> rooms = _roomRepository.GetAllRooms();
            List<Lecturer> lecturers = _lecturerRepository.GetAllLecturers();
            List<Student> students = _studentRepository.GetAllStudents();

            // Go through each room
            for (int i = 0; i < rooms.Count; i++)
            {
                Room room = rooms[i];

                // Assign lecturer to room if found
                foreach (Lecturer lec in lecturers)
                {
                    if (lec.RoomID == room.RoomID)
                    {
                        room.Lecturer = lec;
                        break; // Stop looking after we find the match
                    }
                }

                // Create empty student list for the room
                room.Students = new List<Student>();

                // Add students that belong to the room
                foreach (Student stu in students)
                {
                    if (stu.RoomID == room.RoomID)
                    {
                        room.Students.Add(stu);
                    }
                }
            }

            // Filter rooms if capacity is selected
            if (capacity > 0)
            {
                List<Room> filteredRooms = new List<Room>();

                foreach (Room room in rooms)
                {
                    if (room.Capacity == capacity)
                    {
                        filteredRooms.Add(room);
                    }
                }

                rooms = filteredRooms;
            }

            return View((rooms, capacity));
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

            // Populate occupancy details
            if (room.RoomType == RoomType.Single && room.LecturerID.HasValue)
            {
                room.Lecturer = _lecturerRepository.GetLecturerByID(room.LecturerID.Value);
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
    }
}
