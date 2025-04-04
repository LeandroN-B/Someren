using Microsoft.Data.SqlClient;
using Someren.Models;
using System.Data;

namespace Someren.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly string _connectionString;

        public StudentRepository(IConfiguration configuration)
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
    }
}
