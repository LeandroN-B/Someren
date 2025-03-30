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
        private readonly IStudentRepository _studentRepository;


        public ActivitiesController(IActivityRepository activityRepository, ILecturerRepository lecturerRepository, IStudentRepository studentRepository)
        {
            _activityRepository = activityRepository;
            _lecturerRepository = lecturerRepository;
            _studentRepository = studentRepository;
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
            var activity = _activityRepository.GetActivityByID(activityId);
            var supervisors = _lecturerRepository.GetSupervisorsForActivity(activityId);
            var nonSupervisors = _lecturerRepository.GetNonSupervisorsForActivity(activityId);

            var viewModel = new ActivitySupervisors
            {
                Activity = activity,
                Supervisors = supervisors,
                NonSupervisors = nonSupervisors
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddSupervisor(int activityId, int lecturerId)
        {
            _lecturerRepository.AddSupervisor(activityId, lecturerId);
            return RedirectToAction("ManageSupervisors", new { activityId });
        }

        [HttpGet]
        public IActionResult RemoveSupervisor(int activityId, int lecturerId)
        {
            _lecturerRepository.RemoveSupervisor(activityId, lecturerId);
            return RedirectToAction("ManageSupervisors", new { activityId });
        }



        // ------------------ Participants ------------------ //

        [HttpGet]
        public IActionResult ManageParticipants(int activityId)
        {
            try
            {
                string? message = TempData["Message"] as string;

                Activity activity = _activityRepository.GetActivityByID(activityId);
                List<Student> participants = _studentRepository.GetParticipantsForActivity(activityId);
                List<Student> nonParticipants = _studentRepository.GetNonParticipantsForActivity(activityId);

                ActivityParticipants viewModel = new ActivityParticipants
                {
                    ActivityID = activityId,
                    Activity = activity,
                    Participants = participants,
                    NonParticipants = nonParticipants,
                    ConfirmationMessage = message ?? string.Empty
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return View("Error", $"Error loading participants: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult AddParticipant(int activityId, int studentId)
        {
            try
            {
                _studentRepository.AddParticipant(activityId, studentId);
                TempData["Message"] = "Added: Student was successfully added.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Removed: Error - {ex.Message}";
            }

            return RedirectToAction("ManageParticipants", new { activityId });
        }

        [HttpGet]
        public IActionResult RemoveParticipant(int activityId, int studentId)
        {
            try
            {
                _studentRepository.RemoveParticipant(activityId, studentId);
                TempData["Message"] = "Removed: Student was successfully removed.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Removed: Error - {ex.Message}";
            }

            return RedirectToAction("ManageParticipants", new { activityId });
        }


    }
}
