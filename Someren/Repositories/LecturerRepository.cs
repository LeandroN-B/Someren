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
            List<Lecturer> lecturers = new List<Lecturer>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT lecturerID, firstName, lastName, phoneNumber, dateOfBirth, roomID " +
                               "FROM Lecturer WHERE lastName LIKE @LastName ORDER BY lastName";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@LastName", "%" + lastName + "%");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int lecturerID = Convert.ToInt32(reader["lecturerID"]);
                                string firstName = reader["firstName"]?.ToString() ?? string.Empty;
                                string lastNameValue = reader["lastName"]?.ToString() ?? string.Empty;
                                string phoneNumber = reader["phoneNumber"]?.ToString() ?? string.Empty;
                                DateTime dateOfBirth = Convert.ToDateTime(reader["dateOfBirth"]);
                                int roomID = reader["roomID"] != DBNull.Value ? Convert.ToInt32(reader["roomID"]) : 0;

                                lecturers.Add(new Lecturer(lecturerID, firstName, lastNameValue, phoneNumber, dateOfBirth, roomID));
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Something went wrong with the database", ex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Something went wrong reading data", ex);
                    }
                    //end method refactoring
                }
                return lecturers;
            }
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
                        throw new Exception("A database error occurred while adding the lecturer.", ex);
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
    }
}