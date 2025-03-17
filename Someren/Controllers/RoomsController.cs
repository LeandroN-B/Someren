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

            foreach (var room in rooms)
            {
                room.Lecturer = lecturers.FirstOrDefault(l => l.RoomID == room.RoomID);
            }

            return View(rooms);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Room room)
        {
            try
            {
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
