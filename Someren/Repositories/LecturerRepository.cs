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
                        int roomID = reader["roomID"] != DBNull.Value ? Convert.ToInt32(reader["roomID"]) : 0;

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
                            int roomID = reader["roomID"] != DBNull.Value ? Convert.ToInt32(reader["roomID"]) : 0;

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
                string query = @"UPDATE Lecturer SET firstName = @FirstName, lastName = @LastName, phoneNumber = @PhoneNumber, dateOfBirth = @DateOfBirth, roomID = @RoomID WHERE lecturerID = @LecturerID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LecturerID", lecturer.LecturerID);
                    command.Parameters.AddWithValue("@FirstName", lecturer.FirstName);
                    command.Parameters.AddWithValue("@LastName", lecturer.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", lecturer.PhoneNumber);
                    command.Parameters.AddWithValue("@DateOfBirth", lecturer.DateOfBirth);
                    command.Parameters.AddWithValue("@RoomID", lecturer.RoomID);

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

        public List<Lecturer> GetLecturersByLastName(string lastName)
        {
            List<Lecturer> lecturers = new List<Lecturer>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT lecturerID, firstName, lastName, phoneNumber, dateOfBirth, roomID FROM Lecturer WHERE lastName LIKE @LastName";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@LastName", "%" + lastName + "%"); // supports partial match
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int lecturerID = Convert.ToInt32(reader["lecturerID"]);
                            string firstName = reader["firstName"]?.ToString() ?? string.Empty;
                            string lastNameValue = reader["lastName"]?.ToString() ?? string.Empty;
                            string phoneNumber = reader["phoneNumber"]?.ToString() ?? string.Empty;
                            DateTime dateOfBirth = Convert.ToDateTime(reader["dateOfBirth"]);
                            int roomID = reader["roomID"] != DBNull.Value ? Convert.ToInt32(reader["roomID"]) : 0; // // roomID can be NULL in the database if the lecturer is not assigned a room yet.
                                                                                                              

                            lecturers.Add(new Lecturer(lecturerID, firstName, lastNameValue, phoneNumber, dateOfBirth, roomID));
                        }
                    }
                }
            }

            return lecturers;
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

        public bool CanAssignRoomToLecturer(int lecturerId, int newRoomId)
        {
            // Get the lecturer who is being edited
            Lecturer? existingLecturer = GetLecturerByID(lecturerId);
            if (existingLecturer == null)
                return false;

            // Get the lecturer (if any) already assigned to the desired room
            Lecturer? otherLecturer = GetLecturerByRoomID(newRoomId);

            // Allow if the room is not taken OR is the same one the lecturer already has
            return otherLecturer == null || otherLecturer.LecturerID == lecturerId;
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
