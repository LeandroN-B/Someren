using Someren.Models;

namespace Someren.Repositories
{
    public interface IActivityRepository
    {
        List<Activity> GetAllActivities();
        Activity? GetActivityByID(int id);
        void AddActivity(Activity activity);
        void UpdateActivity(Activity activity);
        void DeleteActivity(Activity activity);
    }
}
