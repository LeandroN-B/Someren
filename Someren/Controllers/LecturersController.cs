using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Someren.Models;
using Someren.Repositories;
using System.Diagnostics.Metrics;

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
           
            string safeLastName = lastName ?? ""; // is guaranteed to lastName never be null,

            List<Lecturer> lecturers = _lecturerRepository.GetAllLecturers(lastName);

            return View(lecturers);
        }



        [HttpGet]
        public IActionResult Create()
        {
            var availableRooms = _roomRepository.GetAvailableSingleRooms(); // only free single rooms
            ViewData["Rooms"] = availableRooms;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Lecturer lecturer)
        {
            {
                if (lecturer.RoomID == null || lecturer.RoomID == 0)
                {
                    ModelState.AddModelError("", "Please select a room.");
                }
                else
                {
                    try
                    {
                        _lecturerRepository.AddLecturer(lecturer);
                        return RedirectToAction("Index");
                    }
                    catch (SqlException ex) when (ex.Message.Contains("UQ_Lecturer_Name")) //is the name of the UNIQUE constraint in the database.
                    {
                        ModelState.AddModelError("", $"A lecturer named {lecturer.FirstName} {lecturer.LastName} already exists.");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error: {ex.Message}");
                    }
                }

                ViewData["Rooms"] = _roomRepository.GetAvailableSingleRooms();
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

            var availableRooms = _roomRepository.GetAvailableSingleRooms(lecturer.RoomID); // also include the current room
            ViewBag.AvailableRooms = availableRooms;

            return View(lecturer);
        }
        [HttpPost]
        public IActionResult Edit(Lecturer lecturer)
        {
            try
            {
                _lecturerRepository.UpdateLecturer(lecturer);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                // fill the ist of options again if something goes wrong
                var availableRooms = _roomRepository.GetAvailableSingleRooms(lecturer.RoomID);
                ViewBag.AvailableRooms = availableRooms;

                return View(lecturer);
            }
        }



    }
}
