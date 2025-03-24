using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;
using System;
using System.Collections.Generic;

namespace Someren.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentRepository _studentRepository;

        public StudentsController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        }

        // Index action to display all students or filter by last name
        public IActionResult Index(string lastName, string sortBy)
        {
            List<Student> students;

            try
            {
                // Retrieve all students
                students = _studentRepository.GetAllStudents();

                // Filter by last name if provided
                if (!string.IsNullOrEmpty(lastName))
                {
                    students = students.FindAll(s => s.LastName.Contains(lastName, StringComparison.OrdinalIgnoreCase));
                }

                // Sort students based on selected option (Last Name by default)
                if (sortBy == "firstName")
                {
                    students = students.OrderBy(s => s.FirstName).ToList();
                }
                else
                {
                    students = students.OrderBy(s => s.LastName).ToList();
                }

                return View(students);
            }
            catch (Exception ex)
            {
                // Handle errors and provide feedback
                TempData["Error"] = $"Error retrieving students: {ex.Message}";
                return View(new List<Student>());
            }
        }

        // Edit Student
        public IActionResult Edit(int id)
        {
            var student = _studentRepository.GetStudentByID(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _studentRepository.UpdateStudent(student);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error updating student: {ex.Message}";
                }
            }

            return View(student);
        }

        // Delete Student
        public IActionResult Delete(int id)
        {
            var student = _studentRepository.GetStudentByID(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var student = _studentRepository.GetStudentByID(id);
                if (student != null)
                {
                    _studentRepository.DeleteStudent(student);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting student: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Show the Create Student form
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // No automatic generation here anymore
                    _studentRepository.AddStudent(student);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error adding student: {ex.Message}";
                }
            }

            return View(student);
        }



    }
}
