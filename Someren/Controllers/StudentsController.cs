using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Someren.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentRepository _studentRepository;

        public StudentsController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public IActionResult Index(string searchString, int? roomId, string sortBy, string sortOrder = "asc", int page = 1, int pageSize = 5)
        {
            var students = _studentRepository.GetAllStudents();

            // Filtering by room ID
            if (roomId.HasValue)
            {
                students = students.Where(s => s.RoomID == roomId.Value).ToList();
            }

            // Searching by name
            if (!string.IsNullOrEmpty(searchString))
            {
                students = students
                    .Where(s => s.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                s.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Sorting logic for A-Z and Z-A
            students = (sortBy, sortOrder) switch
            {
                ("firstName", "asc") => students.OrderBy(s => s.FirstName).ToList(),
                ("firstName", "desc") => students.OrderByDescending(s => s.FirstName).ToList(),
                ("lastName", "asc") => students.OrderBy(s => s.LastName).ToList(),
                ("lastName", "desc") => students.OrderByDescending(s => s.LastName).ToList(),
                _ => students.OrderBy(s => s.LastName).ToList(), // Default sorting
            };

            // Pagination
            int totalStudents = students.Count();
            students = students.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortBy;
            ViewBag.CurrentSortOrder = sortOrder;
            ViewBag.RoomID = roomId;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalStudents / pageSize);
            ViewBag.CurrentPage = page;

            return View(students);
        }
    }
}
