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
            _connectionString = configuration.GetConnectionString("LMBdatabase")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public List<Lecturer> GetAllLecturers()
        {
            List<Lecturer> lecturers = new List<Lecturer>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT lecturerID, firstName, lastName, phoneNumber, dateOfBirth, roomID FROM Lecturer";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int lecturerID = Convert.ToInt32(reader["lecturerID"]);
                        string firstName = reader["firstName"]?.ToString() ?? string.Empty;
                        string lastName = reader["lastName"]?.ToString() ?? string.Empty;
                        string phoneNumber = reader["phoneNumber"]?.ToString() ?? string.Empty;
                        DateTime dateOfBirth = Convert.ToDateTime(reader["dateOfBirth"]);
                        int roomID = reader["roomID"] != DBNull.Value ? Convert.ToInt32(reader["roomID"]) : 0;//handle cases where the roomID is NULL

                        var lecturer = new Lecturer(lecturerID, firstName, lastName, phoneNumber, dateOfBirth, roomID);
                        lecturers.Add(lecturer);
                    }
                }
            }
            return lecturers;
        }

        public Lecturer? GetLecturerByID(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT lecturerID, firstName, lastName, phoneNumber, dateOfBirth, roomID FROM Lecturer WHERE lecturerID = @LecturerID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LecturerID", id);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int lecturerID = Convert.ToInt32(reader["lecturerID"]);
                            string firstName = reader["firstName"].ToString() ?? string.Empty;
                            string lastName = reader["lastName"].ToString() ?? string.Empty;
                            string phoneNumber = reader["phoneNumber"].ToString() ?? string.Empty;
                            DateTime dateOfBirth = Convert.ToDateTime(reader["dateOfBirth"]);
                            int roomID = Convert.ToInt32(reader["roomID"]);

                            return new Lecturer(lecturerID, firstName, lastName, phoneNumber, dateOfBirth, roomID);
                        }
                    }
                }
            }
            return null;
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

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateLecturer(Lecturer lecturer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Lecturer SET firstName = @FirstName, lastName = @LastName, phoneNumber = @PhoneNumber, dateOfBirth = @DateOfBirth WHERE lecturerID = @LecturerID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LecturerID", lecturer.LecturerID);
                    command.Parameters.AddWithValue("@FirstName", lecturer.FirstName);
                    command.Parameters.AddWithValue("@LastName", lecturer.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", lecturer.PhoneNumber);
                    command.Parameters.AddWithValue("@DateOfBirth", lecturer.DateOfBirth);

                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows == 0)
                    {
                        throw new Exception("No records updated!");
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

        public bool IsRoomAvailableForLecturer(int roomId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Lecturer WHERE RoomID = @RoomID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", roomId);
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count == 0;
                }
            }
        }

        public Lecturer? GetLecturerByRoomID(int roomID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT LecturerID, FirstName, LastName, PhoneNumber, DateOfBirth, RoomID FROM Lecturer WHERE RoomID = @RoomID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", roomID);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Lecturer(
                                Convert.ToInt32(reader["LecturerID"]),
                                reader["FirstName"].ToString() ?? string.Empty,
                                reader["LastName"].ToString() ?? string.Empty,
                                reader["PhoneNumber"].ToString() ?? string.Empty,
                                Convert.ToDateTime(reader["DateOfBirth"]),
                                Convert.ToInt32(reader["RoomID"])
                            );
                        }
                    }
                }
            }
            return null;
        }

        public void AssignRoom(int lecturerId, int roomId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Lecturer SET RoomID = @RoomID WHERE LecturerID = @LecturerID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", roomId);
                    command.Parameters.AddWithValue("@LecturerID", lecturerId);

                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows == 0)
                    {
                        throw new Exception("Room assignment failed!");
                    }
                }
            }
        }
    }
}

