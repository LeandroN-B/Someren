using Microsoft.Data.SqlClient;
using Someren.Models;
using System.Data;

namespace Someren.Repositories
{
    public class LecturerRepository : ILecturerRepository
    {
        private readonly string _connectionString;

        public LecturerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("test1database")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public List<Lecturer> GetAllLecturers(string lastName = "")
        {
            string query = "SELECT lecturerID, firstName, lastName, phoneNumber, dateOfBirth, roomID " +
                           "FROM Lecturer WHERE lastName LIKE @LastName ORDER BY lastName";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@LastName", "%" + lastName + "%")
            };

            return ExecuteQueryMapLecturerList(query, parameters);
        }
        public Lecturer? GetLecturerByID(int id)
        {
            string query = "SELECT lecturerID, firstName, lastName, phoneNumber, dateOfBirth, roomID FROM Lecturer WHERE lecturerID = @LecturerID";
            SqlParameter[] sqlParameters =
            {
                new SqlParameter("@LecturerID", SqlDbType.Int) { Value = id }
            };

            return ExecuteQueryMapLecturer(query, sqlParameters);
        }

        public void AddLecturer(Lecturer lecturer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Lecturer (firstName, lastName, phoneNumber, dateOfBirth, roomID) " +
                               "VALUES (@FirstName, @LastName, @PhoneNumber, @DateOfBirth, @RoomID)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", lecturer.FirstName);
                    command.Parameters.AddWithValue("@LastName", lecturer.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", lecturer.PhoneNumber);
                    command.Parameters.AddWithValue("@DateOfBirth", lecturer.DateOfBirth);
                    command.Parameters.AddWithValue("@RoomID", lecturer.RoomID);

                    try
                    {
                        connection.Open();
                        int affectedRows = command.ExecuteNonQuery();

                        if (affectedRows != 1)
                        {
                            throw new Exception("Adding lecturer failed — no row was inserted.");
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw; //no specification here to let my controller check the message directly
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Something went wrong while adding the lecturer.", ex);
                    }
                }
            }
        }
        public void UpdateLecturer(Lecturer lecturer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Lecturer SET firstName = @FirstName, lastName = @LastName, phoneNumber = @PhoneNumber, dateOfBirth = @DateOfBirth, roomID = @RoomID WHERE lecturerID = @LecturerID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LecturerID", lecturer.LecturerID);
                    command.Parameters.AddWithValue("@FirstName", lecturer.FirstName);
                    command.Parameters.AddWithValue("@LastName", lecturer.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", lecturer.PhoneNumber);
                    command.Parameters.AddWithValue("@DateOfBirth", lecturer.DateOfBirth);
                    command.Parameters.AddWithValue("@RoomID", lecturer.RoomID);
                    try
                    {
                        connection.Open();
                        int affectedRows = command.ExecuteNonQuery();
                        if (affectedRows == 0)
                        {
                            throw new Exception("No records updated!");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Something went wrong", ex);
                    }
                }
            }
        }
        public void DeleteLecturer(Lecturer lecturer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Lecturer WHERE lecturerID = @LecturerID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LecturerID", lecturer.LecturerID);

                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows == 0)
                    {
                        throw new Exception("No records deleted!");
                    }
                }
            }
        }
        private Lecturer? ExecuteQueryMapLecturer(string query, SqlParameter[] sqlParameters)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(sqlParameters);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Lecturer(
                                    Convert.ToInt32(reader["lecturerID"]),
                                    reader["firstName"]?.ToString() ?? string.Empty,
                                    reader["lastName"]?.ToString() ?? string.Empty,
                                    reader["phoneNumber"]?.ToString() ?? string.Empty,
                                    Convert.ToDateTime(reader["dateOfBirth"]),
                                    reader["roomID"] != DBNull.Value ? Convert.ToInt32(reader["roomID"]) : 0
                                );
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error while mapping lecturer", ex);
                    }

                    return null;
                }
            }

        }
        private List<Lecturer> ExecuteQueryMapLecturerList(string query, SqlParameter[] parameters)
        {
            List<Lecturer> lecturers = new List<Lecturer>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddRange(parameters);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lecturers.Add(new Lecturer(
                                Convert.ToInt32(reader["lecturerID"]),
                                reader["firstName"]?.ToString() ?? string.Empty,
                                reader["lastName"]?.ToString() ?? string.Empty,
                                reader["phoneNumber"]?.ToString() ?? string.Empty,
                                Convert.ToDateTime(reader["dateOfBirth"]),
                                reader["roomID"] != DBNull.Value ? Convert.ToInt32(reader["roomID"]) : 0
                            ));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Database error while loading lecturers.", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error mapping lecturers from result.", ex);
                }
            }

            return lecturers;
        }
        public List<Lecturer> GetSupervisorsForActivity(int activityId)
        {
            string query = @"
        SELECT l.lecturerID, l.firstName, l.lastName, l.phoneNumber, l.dateOfBirth, l.roomID
        FROM Lecturer l
        INNER JOIN Supervises s ON l.lecturerID = s.lecturerID
        WHERE s.activityID = @ActivityID";

            SqlParameter[] parameters =
            {
        new SqlParameter("@ActivityID", activityId)
    };

            return ExecuteQueryMapLecturerList(query, parameters);
        }
        public List<Lecturer> GetNonSupervisorsForActivity(int activityId)
        {
            string query = @"
        SELECT l.lecturerID, l.firstName, l.lastName, l.phoneNumber, l.dateOfBirth, l.roomID
        FROM Lecturer l
        WHERE l.lecturerID NOT IN (
            SELECT lecturerID FROM Supervises WHERE activityID = @ActivityID
        )";

            SqlParameter[] parameters =
            {
        new SqlParameter("@ActivityID", activityId)
    };

            return ExecuteQueryMapLecturerList(query, parameters);
        }
        public void AddSupervisor(int activityId, int lecturerId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Supervises (activityID, lecturerID) VALUES (@ActivityID, @LecturerID)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ActivityID", activityId);
                    command.Parameters.AddWithValue("@LecturerID", lecturerId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception("Failed to add supervisor.");
                    }
                }
            }
        }
        public void RemoveSupervisor(int activityId, int lecturerId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Supervises WHERE activityID = @ActivityID AND lecturerID = @LecturerID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ActivityID", activityId);
                    command.Parameters.AddWithValue("@LecturerID", lecturerId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception("Failed to remove supervisor.");
                    }
                }
            }
        }

    }

}