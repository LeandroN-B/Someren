using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;

namespace Someren.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentRepository _studentRepository;

        // Dependency injection through constructor
        public StudentsController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public IActionResult Index()
        {
            List<Student> students = _studentRepository.GetAllStudents()
                                         .OrderBy(s => s.LastName) // Sort by Last Name (A-Z)
                                         .ToList();
            return View(students);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Student student)
        {
            try
            {
                _studentRepository.AddStudent(student);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(student);
            }
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Student? student = _studentRepository.GetStudentByID((int)id);
            if (student is null)
            {
                return NotFound();
            }
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

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Student? student = _studentRepository.GetStudentByID((int)id);
            return View(student);
        }

        [HttpPost]
        public IActionResult Edit(Student student)
        {
            try
            {
                _studentRepository.UpdateStudent(student);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(student);
            }
        }
    }
}
