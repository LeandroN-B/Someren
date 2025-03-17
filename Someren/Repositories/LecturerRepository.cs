using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories
{
    public class LecturerRepository : ILecturerRepository
    {
        private readonly string _connectionString;

        //conection with the DB
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
                        int roomID = Convert.ToInt32(reader["roomID"]);

                        // Create the Lecturer object
                        var lecturer = new Lecturer(lecturerID, firstName, lastName, phoneNumber, dateOfBirth, roomID);

                        // Add the Lecturer to the list
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
            return null; // Return null if no lecturer is found
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
                    command.Parameters.AddWithValue("@RoomID", lecturer.RoomID);  // Ensure this is passed correctly

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

    }
}