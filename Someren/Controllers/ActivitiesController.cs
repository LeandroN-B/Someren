using Someren.Models;
using Microsoft.AspNetCore.Mvc;
using Someren.Repositories;
using Microsoft.Data.SqlClient;

namespace Someren.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly IActivityRepository _activityRepository;
        private readonly ILecturerRepository _lecturerRepository;


        public ActivitiesController(IActivityRepository activityRepository, ILecturerRepository lecturerRepository)
        {
            _activityRepository = activityRepository;
            _lecturerRepository = lecturerRepository;
        }

        public IActionResult Index()
        {
            List<Activity> activities = _activityRepository.GetAllActivities();
            return View(activities);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Activity activity)
        {
            if (!ModelState.IsValid)
            {
                return View(activity);
            }

            try
            {
                _activityRepository.AddActivity(activity);
                return RedirectToAction("Index");
            }
            catch (SqlException ex) when (ex.Message.Contains("UQ_Activity_Name"))//Unique name, same strategy used in Lecturer
            {
                ModelState.AddModelError("", "An activity with this name already exists.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }

            return View(activity);

        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            Activity? activity = _activityRepository.GetActivityByID((int)id);
            if (activity == null)
                return NotFound();

            return View(activity);
        }

        [HttpPost]
        public IActionResult Edit(Activity activity)
        {
            if (!ModelState.IsValid)
            {
                return View(activity);
            }

            try
            {
                _activityRepository.UpdateActivity(activity);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(activity);
            }
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            Activity? activity = _activityRepository.GetActivityByID((int)id);
            if (activity == null)
                return NotFound();

            return View(activity);
        }

        [HttpPost]
        public IActionResult Delete(Activity activity)
        {
            try
            {
                _activityRepository.DeleteActivity(activity);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(activity);
            }
        }
        [HttpGet]
        public IActionResult ManageSupervisors(int activityId)
        {
            var viewModel = LoadSupervisorViewModel(activityId);
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddSupervisor(int activityId, int lecturerId)
        {
            _lecturerRepository.AddSupervisor(activityId, lecturerId);
            ViewBag.Message = "Lecturer was successfully added as a supervisor.";
            return View("ManageSupervisors", LoadSupervisorViewModel(activityId));
        }

        [HttpGet]
        public IActionResult RemoveSupervisor(int activityId, int lecturerId)
        {
            _lecturerRepository.RemoveSupervisor(activityId, lecturerId);
            ViewBag.Message = "Lecturer was removed from supervisors.";
            return View("ManageSupervisors", LoadSupervisorViewModel(activityId));
        }
        //to reduce repetition in supervisor logics
        private ActivitySupervisors LoadSupervisorViewModel(int activityId)
        {
            var activity = _activityRepository.GetActivityByID(activityId);
            var supervisors = _lecturerRepository.GetSupervisorsForActivity(activityId);
            var nonSupervisors = _lecturerRepository.GetNonSupervisorsForActivity(activityId);

            return new ActivitySupervisors
            {
                Activity = activity,
                Supervisors = supervisors,
                NonSupervisors = nonSupervisors
            };
        }
    }
}
