using Microsoft.Data.SqlClient;
using Someren.Models;
using System;
using System.Collections.Generic;

namespace Someren.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly string _connectionString;

        public int StudentID { get; private set; }

        public StudentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("LMBdatabase")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public List<Student> GetAllStudents()
        {
            List<Student> students = new List<Student>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT studentID, firstName, lastName, telephone, class FROM Student";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int studentID = Convert.ToInt32(reader["studentID"]);
                        string firstName = reader["firstName"].ToString();
                        string lastName = reader["lastName"].ToString();
                        string phoneNumber = reader["phoneNumber"].ToString();
                        DateTime dateOfBirth = Convert.ToDateTime(reader["dateOfBirth"]);

                        // Create the Student object
                        var lecturer = new Student(StudentID, firstName, lastName, phoneNumber, dateOfBirth);

                        // Add the Lecturer to the list
                        Student.Add(student);

                    }
                }
            }
            return students;
        }

        public Student? GetStudentByID(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT studentID, firstName, lastName, telephone, class FROM Student WHERE studentID = @StudentID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", id);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int studentID = Convert.ToInt32(reader["studentID"]);
                            string firstName = reader["firstName"].ToString();
                            string lastName = reader["lastName"].ToString();
                            string telephone = reader["telephone"].ToString();
                            string className = reader["class"].ToString();

                            return new Student(studentID, firstName, lastName, telephone, className);
                        }
                    }
                }
            }
            return null;
        }

        public void AddStudent(Student student)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Student (firstName, lastName, telephone, class) VALUES (@FirstName, @LastName, @Telephone, @Class)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@Telephone", student.Telephone);
                    command.Parameters.AddWithValue("@Class", student.Class);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateStudent(Student student)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Student SET firstName = @FirstName, lastName = @LastName, telephone = @Telephone, class = @Class WHERE studentID = @StudentID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", student.StudentID);
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@Telephone", student.Telephone);
                    command.Parameters.AddWithValue("@Class", student.Class);

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
    }
}
