using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;

namespace Someren.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomRepository _roomRepository;
        private readonly ILecturerRepository _lecturerRepository;

        public RoomsController(IRoomRepository roomRepository, ILecturerRepository lecturerRepository)
        {
            _roomRepository = roomRepository;
            _lecturerRepository = lecturerRepository;
        }

        public IActionResult Index()
        {
            List<Room> rooms = _roomRepository.GetAllRooms();
            List<Lecturer> lecturers = _lecturerRepository.GetAllLecturers();

            foreach (Room room in rooms)
            {
                room.Lecturer = lecturers.FirstOrDefault(l => l.RoomID == room.RoomID);
            }

            return View(rooms);
        }

        [HttpGet]
        public IActionResult Create()
        {
            List<Lecturer> lecturers = _lecturerRepository.GetAllLecturers();
            List<Room> rooms = _roomRepository.GetAllRooms();

            // Get lecturers who are not assigned to any room
            var availableLecturers = lecturers
                .Where(l => !rooms.Any(r => r.RoomID == l.RoomID))
                .ToList();

            ViewBag.Lecturers = availableLecturers;
            return View(new Room());
        }


        [HttpPost]
        public IActionResult Create(Room room, int? LecturerID)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Lecturers = _lecturerRepository.GetAllLecturers();
                    return View(room);
                }

                _roomRepository.AddRoom(room);

                if (room.RoomType == RoomType.Single && LecturerID.HasValue)
                {
                    _lecturerRepository.AssignRoom(LecturerID.Value, room.RoomID);
                }

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.Lecturers = _lecturerRepository.GetAllLecturers();
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

            // Get and assing the lecturer if the room is a single room
            if (room.RoomType == RoomType.Single)
            {
                Lecturer? lecturer = _lecturerRepository.GetLecturerByRoomID(room.RoomID);
                room.Lecturer = lecturer;
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
