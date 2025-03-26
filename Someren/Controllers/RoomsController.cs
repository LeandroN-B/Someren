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
            List<Room> rooms = _roomRepository.GetRoomsWithPeople(capacity);

            if (capacity != 1 && capacity != 8)
                capacity = 0;

            return View((rooms, capacity));
        }



        // Shows the Create Room form
        [HttpGet]
        public IActionResult Create()
        {
            // Returns an empty Room object to fill in the form
            return View(new Room());
        }

        [HttpPost]
        public IActionResult Create(Room room)
        {
            if (!ModelState.IsValid)
                return View(room);

            try
            {
                _roomRepository.AddRoomIfNotExists(room);
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("RoomNumber", ex.Message);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
            }

            return View(room);
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

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id is null) return NotFound();

            Room? room = _roomRepository.GetRoomByID((int)id);
            if (room == null) return NotFound();

            _roomRepository.LoadOccupantsForRoom(room);

            return View(room);
        }
             

        [HttpGet]
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id is null) return NotFound();

            Room? room = _roomRepository.GetRoomByID((int)id);
            if (room == null) return NotFound();

            _roomRepository.LoadOccupantsForRoom(room);

            return View(room);
        }

    }
}
