using Someren.Models;
using Microsoft.AspNetCore.Mvc;
using Someren.Repositories;

namespace Someren.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly IActivityRepository _activityRepository;

        public ActivitiesController(IActivityRepository activityRepository)
        {
            _activityRepository = activityRepository;
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
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(activity);
            }
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
    }
}
