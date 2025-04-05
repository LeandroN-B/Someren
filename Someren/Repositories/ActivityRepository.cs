using Someren.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Someren.Repositories
{

    public class ActivityRepository : IActivityRepository
    {
        private readonly string _connectionString;

        public ActivityRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("test1database")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        // Get all activities ordered by date and start time
        public List<Activity> GetAllActivities()
        {
            string query = "SELECT activityID, activityName, activityDate, timeOfDay, startTime, endTime FROM Activity ORDER BY activityDate, startTime";
            return ExecuteQueryMapActivityList(query);
        }

        public Activity? GetActivityByID(int id)
        {
            string query = "SELECT activityID, activityName, activityDate, timeOfDay, startTime, endTime FROM Activity WHERE activityID = @ActivityID";
            SqlParameter[] parameters =
            {
                new SqlParameter("@ActivityID", SqlDbType.Int) { Value = id }
            };

            return ExecuteQueryMapActivity(query, parameters);
        }

        public void AddActivity(Activity activity)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = "INSERT INTO Activity (activityName, activityDate, timeOfDay, startTime, endTime) " +
                           "VALUES (@ActivityName, @ActivityDate, @TimeOfDay, @StartTime, @EndTime)";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ActivityName", activity.ActivityName);
            command.Parameters.AddWithValue("@ActivityDate", activity.ActivityDate);
            command.Parameters.AddWithValue("@TimeOfDay", activity.TimeOfDay);
            command.Parameters.AddWithValue("@StartTime", activity.StartTime);
            command.Parameters.AddWithValue("@EndTime", activity.EndTime);

            connection.Open();
            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows != 1)
            {
                throw new Exception("Failed to insert activity.");
            }
        }

        public void UpdateActivity(Activity activity)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"UPDATE Activity SET activityName = @ActivityName, activityDate = @ActivityDate, 
                             timeOfDay = @TimeOfDay, startTime = @StartTime, endTime = @EndTime WHERE activityID = @ActivityID";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ActivityID", activity.ActivityID);
            command.Parameters.AddWithValue("@ActivityName", activity.ActivityName);
            command.Parameters.AddWithValue("@ActivityDate", activity.ActivityDate);
            command.Parameters.AddWithValue("@TimeOfDay", activity.TimeOfDay);
            command.Parameters.AddWithValue("@StartTime", activity.StartTime);
            command.Parameters.AddWithValue("@EndTime", activity.EndTime);

            connection.Open();
            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows == 0)
            {
                throw new Exception("No records updated.");
            }
        }

        public void DeleteActivity(Activity activity)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = "DELETE FROM Activity WHERE activityID = @ActivityID";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ActivityID", activity.ActivityID);

            connection.Open();
            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows == 0)
            {
                throw new Exception("No records deleted.");
            }
        }

        private Activity? ExecuteQueryMapActivity(string query, SqlParameter[] parameters)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Activity(
                    Convert.ToInt32(reader["activityID"]),
                    reader["activityName"].ToString() ?? string.Empty,
                    Convert.ToDateTime(reader["activityDate"]),
                    reader["timeOfDay"].ToString() ?? string.Empty,
                    (TimeSpan)reader["startTime"],
                    (TimeSpan)reader["endTime"]
                );
            }

            return null;
        }

        private List<Activity> ExecuteQueryMapActivityList(string query)
        {
            List<Activity> activities = new();
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                activities.Add(new Activity(
                    Convert.ToInt32(reader["activityID"]),
                    reader["activityName"].ToString() ?? string.Empty,
                    Convert.ToDateTime(reader["activityDate"]),
                    reader["timeOfDay"].ToString() ?? string.Empty,
                    (TimeSpan)reader["startTime"],
                    (TimeSpan)reader["endTime"]
                ));
            }

            return activities;
        }
    }
}


