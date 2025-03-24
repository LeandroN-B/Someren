using Microsoft.Data.SqlClient;
using Someren.Models;
using System;
using System.Collections.Generic;

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

        public List<Student> GetAllStudents()
        {
            List<Student> students = new List<Student>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT studentID, studentNumber, firstName, lastName, phoneNumber, class, roomID FROM Student";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(new Student(
                        reader["studentID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["studentID"]),
                        reader["studentNumber"] == DBNull.Value ? string.Empty : reader["studentNumber"].ToString(),
                        reader["firstName"] == DBNull.Value ? string.Empty : reader["firstName"].ToString(),
                        reader["lastName"] == DBNull.Value ? string.Empty : reader["lastName"].ToString(),
                        reader["phoneNumber"] == DBNull.Value ? string.Empty : reader["phoneNumber"].ToString(),
                        reader["class"] == DBNull.Value ? string.Empty : reader["class"].ToString(),
                        reader["roomID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["roomID"])
                       ));
                    }
                }
            }

            return students;
        }

        // Add the GetStudentsByLastName method here
        public List<Student> GetStudentsByLastName(string lastName)
        {
            List<Student> students = new List<Student>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT studentID, studentNumber, firstName, lastName, phoneNumber, class, roomID FROM Student WHERE lastName LIKE @LastName";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@LastName", "%" + lastName + "%");
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(new Student(
                                reader["studentID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["studentID"]),
                                reader["studentNumber"] == DBNull.Value ? string.Empty : reader["studentNumber"].ToString(),
                                reader["firstName"] == DBNull.Value ? string.Empty : reader["firstName"].ToString(),
                                reader["lastName"] == DBNull.Value ? string.Empty : reader["lastName"].ToString(),
                                reader["phoneNumber"] == DBNull.Value ? string.Empty : reader["phoneNumber"].ToString(),
                                reader["class"] == DBNull.Value ? string.Empty : reader["class"].ToString(),
                                reader["roomID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["roomID"])
                            ));
                        }
                    }
                }
            }

            return students;
        }

        public void AddStudent(Student student)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Student (studentNumber, firstName, lastName, phoneNumber, class, roomID) VALUES (@StudentNumber, @FirstName, @LastName, @PhoneNumber, @Class, @RoomID)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
                    command.Parameters.AddWithValue("@Class", student.ClassName);
                    command.Parameters.AddWithValue("@RoomID", student.RoomID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateStudent(Student student)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Student SET studentNumber = @StudentNumber, firstName = @FirstName, lastName = @LastName, phoneNumber = @PhoneNumber, class = @Class, roomID = @RoomID WHERE StudentID = @StudentID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", student.StudentID);
                    command.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
                    command.Parameters.AddWithValue("@Class", student.ClassName);
                    command.Parameters.AddWithValue("@RoomID", student.RoomID);

                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows == 0)
                    {
                        throw new Exception("No records updated!");
                    }
                }
            }
        }

        public void DeleteStudent(Student student)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Student WHERE studentID = @StudentID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", student.StudentID);
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows == 0)
                    {
                        throw new Exception("No records deleted!");
                    }
                }
            }
        }

        public Student? GetStudentByID(int id)
        {
            throw new NotImplementedException();
        }

        public List<Student> GetStudentsByRoomID(int roomId)
        {
            throw new NotImplementedException();
        }
    }
}
