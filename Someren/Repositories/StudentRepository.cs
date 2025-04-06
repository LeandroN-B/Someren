using Microsoft.Data.SqlClient;
using Someren.Models;
using System.Data;

namespace Someren.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly string _connectionString;
        private readonly IActivityRepository _activityRepository;

        public StudentRepository(IConfiguration configuration, IActivityRepository activityRepository)
        {
            _connectionString = configuration.GetConnectionString("test1database")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public List<Student> GetAllStudents(string lastName = "")
        {
            string query = @"SELECT studentID, studentNumber, firstName, lastName, phoneNumber, class, roomID, Vouchers 
                             FROM Student 
                             WHERE lastName LIKE @LastName 
                             ORDER BY lastName";

            SqlParameter[] parameters = {
                new SqlParameter("@LastName", SqlDbType.NVarChar) { Value = "%" + lastName + "%" }
            };

            return ExecuteQueryMapStudents(query, parameters);
        }

        public Student? GetStudentByID(int id)
        {
            string query = @"SELECT studentID, studentNumber, firstName, lastName, phoneNumber, class, roomID, Vouchers 
                             FROM Student 
                             WHERE studentID = @StudentID";

            SqlParameter[] parameters = {
                new SqlParameter("@StudentID", SqlDbType.Int) { Value = id }
            };

            return ExecuteQueryMapStudent(query, parameters);
        }

        public List<Student> GetStudentsByRoomID(int roomId)
        {
            string query = @"SELECT studentID, studentNumber, firstName, lastName, phoneNumber, class, roomID, Vouchers 
                             FROM Student 
                             WHERE roomID = @RoomID";

            SqlParameter[] parameters = {
                new SqlParameter("@RoomID", SqlDbType.Int) { Value = roomId }
            };

            return ExecuteQueryMapStudents(query, parameters);
        }

        public void AddStudent(Student student)
        {
            string query = @"INSERT INTO Student (studentNumber, firstName, lastName, phoneNumber, class, roomID) 
                             VALUES (@StudentNumber, @FirstName, @LastName, @PhoneNumber, @Class, @RoomID)";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
            command.Parameters.AddWithValue("@FirstName", student.FirstName);
            command.Parameters.AddWithValue("@LastName", student.LastName);
            command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
            command.Parameters.AddWithValue("@Class", student.ClassName);
            command.Parameters.AddWithValue("@RoomID", student.RoomID ?? (object)DBNull.Value);

            connection.Open();
            if (command.ExecuteNonQuery() != 1)
                throw new Exception("Insert failed: no rows affected.");
        }

        public void UpdateStudent(Student student)
        {
            string query = @"UPDATE Student 
                             SET studentNumber = @StudentNumber, 
                                 firstName = @FirstName, 
                                 lastName = @LastName, 
                                 phoneNumber = @PhoneNumber, 
                                 class = @Class, 
                                 roomID = @RoomID 
                             WHERE studentID = @StudentID";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@StudentID", student.StudentID);
            command.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
            command.Parameters.AddWithValue("@FirstName", student.FirstName);
            command.Parameters.AddWithValue("@LastName", student.LastName);
            command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
            command.Parameters.AddWithValue("@Class", student.ClassName);
            command.Parameters.AddWithValue("@RoomID", student.RoomID ?? (object)DBNull.Value);

            connection.Open();
            if (command.ExecuteNonQuery() == 0)
                throw new Exception("Update failed: no rows affected.");
        }

        public void UseVouchers(int studentId, int quantity)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "UPDATE Student SET Vouchers = Vouchers - @quantity WHERE StudentID = @studentId";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@quantity", quantity);
            cmd.Parameters.AddWithValue("@studentId", studentId);

            cmd.ExecuteNonQuery();
        }

        public void AddVouchers(int studentId, int amount)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "UPDATE Student SET Vouchers = Vouchers + @amount WHERE StudentID = @studentId";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@studentId", studentId);

            cmd.ExecuteNonQuery();
        }

        public void DeleteStudent(Student student)
        {
            string query = "DELETE FROM Student WHERE studentID = @StudentID";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@StudentID", student.StudentID);

            connection.Open();
            if (command.ExecuteNonQuery() == 0)
                throw new Exception("Delete failed: no rows affected.");
        }

        public List<Student> GetStudentsWithRoomAttached(string lastName = "")
        {
            var students = new List<Student>();

            string query = @"SELECT s.StudentID, s.StudentNumber, s.FirstName, s.LastName, 
                                    s.PhoneNumber, s.Class, s.RoomID, s.Vouchers, r.RoomNumber
                             FROM Student s
                             LEFT JOIN Room r ON s.RoomID = r.RoomID
                             WHERE s.RoomID IS NOT NULL AND s.LastName LIKE @LastName
                             ORDER BY s.LastName";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LastName", "%" + lastName + "%");

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var student = MapStudent(reader);

                if (!reader.IsDBNull(reader.GetOrdinal("RoomID")))
                {
                    student.Room = new Room
                    {
                        RoomID = reader.GetInt32(reader.GetOrdinal("RoomID")),
                        RoomNumber = reader["RoomNumber"]?.ToString() ?? ""
                    };
                }

                students.Add(student);
            }

            return students;
        }

        private Student MapStudent(SqlDataReader reader)
        {
            return new Student(
                studentID: reader.GetInt32(reader.GetOrdinal("studentID")),
                studentNumber: reader["studentNumber"]?.ToString() ?? "",
                firstName: reader["firstName"]?.ToString() ?? "",
                lastName: reader["lastName"]?.ToString() ?? "",
                phoneNumber: reader["phoneNumber"]?.ToString() ?? "",
                className: reader["class"]?.ToString() ?? "",
                roomID: reader.IsDBNull(reader.GetOrdinal("roomID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("roomID")),
                vouchers: reader.IsDBNull(reader.GetOrdinal("Vouchers")) ? 0 : reader.GetInt32(reader.GetOrdinal("Vouchers"))
            );
        }

        private Student? ExecuteQueryMapStudent(string query, SqlParameter[] parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapStudent(reader);
            }

            return null;
        }

        private List<Student> ExecuteQueryMapStudents(string query, SqlParameter[]? parameters = null)
        {
            var students = new List<Student>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                students.Add(MapStudent(reader));
            }

            return students;
        }




        // ------------------ Start Of Managing Participants ------------------ //


        // Gets all students participating in the given activity.
        // Uses INNER JOIN on the Participates table.
        public List<Student> GetParticipantsForActivity(int activityId)
        {
            string query = @"SELECT s.studentID, s.studentNumber, s.firstName, s.lastName, s.phoneNumber, s.class, s.roomID, s.Vouchers
                 FROM Participates p
                 INNER JOIN Student s ON s.StudentID = p.StudentID
                 WHERE p.ActivityID = @ActivityID";

            return ExecuteReaderWithActivityId(query, activityId);
        }

        // Gets all students not participating in the given activity.
        // Uses LEFT JOIN on the Participates table.
        // Uses NOT EXISTS to filter out students who are already participating.
        public List<Student> GetNonParticipantsForActivity(int activityId)
        {
            string query = @"SELECT s.studentID, s.studentNumber, s.firstName, s.lastName, s.phoneNumber, s.class, s.roomID, s.vouchers
                     FROM Student s
                     WHERE NOT EXISTS (
                                        SELECT 1 FROM Participates p
                                        WHERE p.StudentID = s.StudentID AND p.ActivityID = @ActivityID)";

            return ExecuteReaderWithActivityId(query, activityId);
        }


        // Adds a student to the activity's participants.
        // Uses an INSERT statement on the Participates table.
        // Uses parameters to prevent SQL injection.
        public void AddParticipant(int activityId, int studentId)
        {
            string query = "INSERT INTO Participates (ActivityID, StudentID) VALUES (@ActivityID, @StudentID)";
            ExecuteNonQueryWithActivityAndStudent(query, activityId, studentId);
        }


        // Removes a student from the activity's participants.
        // Uses a DELETE statement on the Participates table.
        public void RemoveParticipant(int activityId, int studentId)
        {
            string query = "DELETE FROM Participates WHERE ActivityID = @ActivityID AND StudentID = @StudentID";
            ExecuteNonQueryWithActivityAndStudent(query, activityId, studentId);
        }

        // Gets all students participating in the given activity.
        // Uses INNER JOIN on the Participates table.
        // Uses LEFT JOIN to get the activity details.
        // Returns an ActivityParticipants object containing the activity and the list of participants
        public ActivityParticipantsViewModel GetActivityParticipants(int activityId, string message)
        {
            ActivityParticipantsViewModel result = new ActivityParticipantsViewModel();

            result.ActivityID = activityId;
            result.Activity = _activityRepository.GetActivityByID(activityId);
            result.Participants = GetParticipantsForActivity(activityId);
            result.NonParticipants = GetNonParticipantsForActivity(activityId);

            if (message != null)
            {
                result.ConfirmationMessage = message;
            }
            else
            {
                result.ConfirmationMessage = string.Empty;
            }

            return result;
        }



        //  ---- Private Methods (Re-usable codes) ---- //


        // Executes a SELECT query with ActivityID to fetch matching students.
        // Maps the results to a list of Student objects.
        private List<Student> ExecuteReaderWithActivityId(string query, int activityId)
        {
            List<Student> students = new List<Student>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ActivityID", activityId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Student student = MapStudent(reader);
                            students.Add(student);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("An error occurred while retrieving students for the activity.", ex);
            }

            return students;
        }


        // Executes an INSERT or DELETE with ActivityID and StudentID.
        // Used for adding/removing participants.
        private void ExecuteNonQueryWithActivityAndStudent(string query, int activityId, int studentId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ActivityID", activityId);
                    command.Parameters.AddWithValue("@StudentID", studentId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception("No rows were affected. Participant may already exist or not be found.");
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("An error occurred while updating participant data.", ex);
            }
        }

        // ------------------ END Of Managing Participants ------------------ //ts
    }
}
