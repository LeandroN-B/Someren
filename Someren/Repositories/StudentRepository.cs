using Microsoft.Data.SqlClient;
using Someren.Models;
using System;
using System.Collections.Generic;
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
            string query = @"SELECT studentID, studentNumber, firstName, lastName, phoneNumber, class, roomID 
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
            string query = @"SELECT studentID, studentNumber, firstName, lastName, phoneNumber, class, roomID 
                             FROM Student 
                             WHERE studentID = @StudentID";

            SqlParameter[] parameters = {
                new SqlParameter("@StudentID", SqlDbType.Int) { Value = id }
            };

            return ExecuteQueryMapStudent(query, parameters);
        }

        public List<Student> GetStudentsByRoomID(int roomId)
        {
            string query = @"SELECT studentID, studentNumber, firstName, lastName, phoneNumber, class, roomID 
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

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
                command.Parameters.AddWithValue("@FirstName", student.FirstName);
                command.Parameters.AddWithValue("@LastName", student.LastName);
                command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
                command.Parameters.AddWithValue("@Class", student.ClassName);
                command.Parameters.AddWithValue("@RoomID", student.RoomID ?? (object)DBNull.Value);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected != 1)
                        throw new Exception("Insert failed: no rows affected.");
                }
                catch (SqlException ex)
                {
                    throw new Exception("SQL error while adding student.", ex);
                }
            }
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

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@StudentID", student.StudentID);
                command.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
                command.Parameters.AddWithValue("@FirstName", student.FirstName);
                command.Parameters.AddWithValue("@LastName", student.LastName);
                command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
                command.Parameters.AddWithValue("@Class", student.ClassName);
                command.Parameters.AddWithValue("@RoomID", student.RoomID ?? (object)DBNull.Value);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                        throw new Exception("Update failed: no rows affected.");
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while updating student.", ex);
                }
            }
        }

        public void DeleteStudent(Student student)
        {
            string query = @"DELETE FROM Student WHERE studentID = @StudentID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@StudentID", student.StudentID);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    throw new Exception("Delete failed: no rows affected.");
            }
        }

        private Student? ExecuteQueryMapStudent(string query, SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddRange(parameters);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapStudent(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error reading single student record.", ex);
                }

                return null;
            }
        }

        private List<Student> ExecuteQueryMapStudents(string query, SqlParameter[]? parameters = null)
        {
            List<Student> students = new List<Student>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(MapStudent(reader));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error reading student list.", ex);
                }
            }

            return students;
        }

        private Student MapStudent(SqlDataReader reader)
        {
            return new Student(
                Convert.ToInt32(reader["studentID"]),
                reader["studentNumber"]?.ToString() ?? "",
                reader["firstName"]?.ToString() ?? "",
                reader["lastName"]?.ToString() ?? "",
                reader["phoneNumber"]?.ToString() ?? "",
                reader["class"]?.ToString() ?? "",
                reader["roomID"] != DBNull.Value ? Convert.ToInt32(reader["roomID"]) : (int?)null
            );
        }

        public List<Student> GetStudentsWithRoomAttached(string lastName = "")
        {
            List<Student> students = new List<Student>();

            string query = @"SELECT s.StudentID, s.StudentNumber, s.FirstName, s.LastName, 
                                    s.PhoneNumber, s.Class, s.RoomID, r.RoomNumber
                             FROM Student s
                             LEFT JOIN Room r ON s.RoomID = r.RoomID
                             WHERE s.RoomID IS NOT NULL AND s.LastName LIKE @LastName
                             ORDER BY s.LastName";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@LastName", "%" + lastName + "%");

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Student student = MapStudent(reader);
                            student.Room = new Room
                            {
                                RoomID = reader["RoomID"] != DBNull.Value ? Convert.ToInt32(reader["RoomID"]) : 0,
                                RoomNumber = reader["RoomNumber"]?.ToString() ?? ""
                            };
                            students.Add(student);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error retrieving students with room data.", ex);
                }
            }

            return students;
        }
    }
}
