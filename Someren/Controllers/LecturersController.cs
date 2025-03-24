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

            if (!string.IsNullOrEmpty(lastName))
            {
                var filteredLecturers = _lecturerRepository.GetLecturersByLastName(lastName);
                return View(filteredLecturers);
            }

            var allLecturers = _lecturerRepository.GetAllLecturers()
                                                  .OrderBy(l => l.LastName)
                                                  .ToList();
            return View(allLecturers);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            Lecturer? lecturer = _lecturerRepository.GetLecturerByID((int)id);
            if (lecturer == null)
                return NotFound();

            var availableRooms = _roomRepository.GetAllRooms()
                .Where(r => r.RoomType == RoomType.Single &&
                            (_lecturerRepository.GetLecturerByRoomID(r.RoomID) == null || r.RoomID == lecturer.RoomID))
                .ToList();

            ViewBag.AvailableRooms = availableRooms;

            return View(lecturer);
        }

        [HttpPost]
        public IActionResult Edit(Lecturer lecturer)
        {
            try
            {
                if (!_lecturerRepository.CanAssignRoomToLecturer(lecturer.LecturerID, lecturer.RoomID))
                {
                    ModelState.AddModelError("", "Selected room is not available or invalid.");

                    var availableRooms = _roomRepository.GetAllRooms()
                        .Where(r => r.RoomType == RoomType.Single &&
                                    (_lecturerRepository.GetLecturerByRoomID(r.RoomID) == null || r.RoomID == lecturer.RoomID))
                        .ToList();

                    ViewBag.AvailableRooms = availableRooms;
                    return View(lecturer);
                }

                _lecturerRepository.UpdateLecturer(lecturer);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                var availableRooms = _roomRepository.GetAllRooms()
                    .Where(r => r.RoomType == RoomType.Single &&
                                (_lecturerRepository.GetLecturerByRoomID(r.RoomID) == null || r.RoomID == lecturer.RoomID))
                    .ToList();

                ViewBag.AvailableRooms = availableRooms;
                return View(lecturer);
            }
        }
    }
}
