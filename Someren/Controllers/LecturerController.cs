using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;

namespace Someren.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ILecturerRepository _lecturerRepository;

        //the dependencies should be injected through a ctor
        public LecturerController(ILecturerRepository lecturerRepository)
        {
            _lecturerRepository = lecturerRepository;
        }

        public IActionResult Index()
        {
            List<Lecturer> lecturers = _lecturerRepository.GetAllLecturers();
            return View(lecturers);
        }            

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Lecturer lecturer)
        {
            try
            {
                _lecturerRepository.AddLecturer(lecturer);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
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
            if (id is null)
            {
                return NotFound();
            }

            Lecturer? lecturer = _lecturerRepository.GetLecturerByID((int)id);
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
            catch (Exception)
            {
                return View(lecturer);
            }
        }
        
    }
}
