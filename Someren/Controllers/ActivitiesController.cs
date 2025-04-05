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



        // ------------------ Start Of Participants ------------------ //


        // Displays the participants and non-participants of an activity.
        // Also shows confirmation messages after add/remove actions.
        [HttpGet]
        public IActionResult ManageParticipants(int activityId)
        {
            string? message = TempData["Message"] as string;

            Activity? activity = _activityRepository.GetActivityByID(activityId);
            if (activity == null)
            {
                TempData["Message"] = "Error: Activity not found.";
                return RedirectToAction("Index");
            }

            List<Student> participants = _studentRepository.GetParticipantsForActivity(activityId);
            List<Student> nonParticipants = _studentRepository.GetNonParticipantsForActivity(activityId);

            ActivityParticipantsViewModel viewModel = new ActivityParticipantsViewModel
            {
                ActivityID = activityId,
                Activity = activity,
                Participants = participants,
                NonParticipants = nonParticipants,
                ConfirmationMessage = message ?? string.Empty
            };

            return View(viewModel);
        }


        // Adds a student to the activity's participants.
        // Shows a success message with the student's name.
        [HttpGet]
        public IActionResult AddParticipant(int activityId, int studentId)
        {
            try
            {
                Student? student = _studentRepository.GetStudentByID(studentId);
                _studentRepository.AddParticipant(activityId, studentId);

                if (student != null)
                {
                    TempData["Message"] = $"Added: {student.FirstName} {student.LastName} was successfully added.";
                }
                else
                {
                    TempData["Message"] = "Added: Student was successfully added.";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Removed: Error - {ex.Message}";
            }

            return RedirectToAction("ManageParticipants", new { activityId });
        }


        // Removes a student from the activity's participants.
        // Shows a success message with the student's name.
        [HttpGet]
        public IActionResult RemoveParticipant(int activityId, int studentId)
        {
            try
            {
                Student? student = _studentRepository.GetStudentByID(studentId);
                _studentRepository.RemoveParticipant(activityId, studentId);

                if (student != null)
                {
                    TempData["Message"] = $"Removed: {student.FirstName} {student.LastName} was successfully removed.";
                }
                else
                {
                    TempData["Message"] = "Removed: Student was successfully removed.";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Removed: Error - {ex.Message}";
            }

            return RedirectToAction("ManageParticipants", new { activityId });
        }


        // ------------------ End Of Participants ------------------ //
    }
}
