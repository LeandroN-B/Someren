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
            List<Student> students = new List<Student>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT studentID, studentNumber, firstName, lastName, phoneNumber, class, roomID " +
                               "FROM Student WHERE lastName LIKE @LastName ORDER BY lastName";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@LastName", "%" + lastName + "%");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int studentID = Convert.ToInt32(reader["studentID"]);
                                string studentNumber = reader["studentNumber"]?.ToString() ?? string.Empty;
                                string firstName = reader["firstName"]?.ToString() ?? string.Empty;
                                string lastNameValue = reader["lastName"]?.ToString() ?? string.Empty;
                                string phoneNumber = reader["phoneNumber"]?.ToString() ?? string.Empty;
                                string className = reader["class"]?.ToString() ?? string.Empty;
                                int? roomID = reader["roomID"] != DBNull.Value ? Convert.ToInt32(reader["roomID"]) : (int?)null;

                                students.Add(new Student(studentID, studentNumber, firstName, lastNameValue, phoneNumber, className, roomID));
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
                }
            }

            return students;
        }

        public Student? GetStudentByID(int id)
        {
            string query = "SELECT studentID, studentNumber, firstName, lastName, phoneNumber, class, roomID FROM Student WHERE studentID = @StudentID";
            SqlParameter[] parameters = {
                new SqlParameter("@StudentID", SqlDbType.Int) { Value = id }
            };
            return ExecuteQueryMapStudent(query, parameters);
        }

        public List<Student> GetStudentsByRoomID(int roomId)
        {
            string query = "SELECT studentID, studentNumber, firstName, lastName, phoneNumber, class, roomID FROM Student WHERE roomID = @RoomID";
            SqlParameter[] parameters = {
                new SqlParameter("@RoomID", SqlDbType.Int) { Value = roomId }
            };
            return ExecuteQueryMapStudents(query, parameters);
        }

        public void AddStudent(Student student)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Student (studentNumber, firstName, lastName, phoneNumber, class, roomID) " +
                               "VALUES (@StudentNumber, @FirstName, @LastName, @PhoneNumber, @Class, @RoomID)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
                    command.Parameters.AddWithValue("@Class", student.ClassName);
                    command.Parameters.AddWithValue("@RoomID", student.RoomID == null ? DBNull.Value : (object)student.RoomID);

                    try
                    {
                        connection.Open();
                        int affectedRows = command.ExecuteNonQuery();

                        if (affectedRows != 1)
                        {
                            throw new Exception("Adding student failed � no row was inserted.");
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("A database error occurred while adding the student.", ex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Something went wrong while adding the student.", ex);
                    }
                }
            }
        }

        public void UpdateStudent(Student student)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Student SET studentNumber = @StudentNumber, firstName = @FirstName, lastName = @LastName, phoneNumber = @PhoneNumber, class = @Class, roomID = @RoomID WHERE studentID = @StudentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", student.StudentID);
                    command.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
                    command.Parameters.AddWithValue("@Class", student.ClassName);
                    command.Parameters.AddWithValue("@RoomID", student.RoomID == null ? DBNull.Value : (object)student.RoomID);

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

        private Student? ExecuteQueryMapStudent(string query, SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
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
                                return new Student(
                                    Convert.ToInt32(reader["studentID"]),
                                    reader["studentNumber"]?.ToString() ?? string.Empty,
                                    reader["firstName"]?.ToString() ?? string.Empty,
                                    reader["lastName"]?.ToString() ?? string.Empty,
                                    reader["phoneNumber"]?.ToString() ?? string.Empty,
                                    reader["class"]?.ToString() ?? string.Empty,
                                    reader["roomID"] != DBNull.Value ? Convert.ToInt32(reader["roomID"]) : (int?)null
                                );
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error while mapping student", ex);
                    }

                    return null;
                }
            }
        }

        private List<Student> ExecuteQueryMapStudents(string query, SqlParameter[]? parameters = null)
        {
            List<Student> students = new List<Student>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(new Student(
                                Convert.ToInt32(reader["studentID"]),
                                reader["studentNumber"]?.ToString() ?? string.Empty,
                                reader["firstName"]?.ToString() ?? string.Empty,
                                reader["lastName"]?.ToString() ?? string.Empty,
                                reader["phoneNumber"]?.ToString() ?? string.Empty,
                                reader["class"]?.ToString() ?? string.Empty,
                                reader["roomID"] != DBNull.Value ? Convert.ToInt32(reader["roomID"]) : (int?)null
                            ));
                        }
                    }
                }
            }
            return students;
        }


        public List<Student> GetStudentsWithRoomAttached(string lastName = "")
        {
            List<Student> students = new List<Student>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT s.StudentID, s.StudentNumber, s.FirstName, s.LastName, 
                                s.PhoneNumber, s.Class, s.RoomID,
                                r.RoomNumber
                         FROM Student s
                         LEFT JOIN Room r ON s.RoomID = r.RoomID
                         WHERE s.RoomID IS NOT NULL AND s.LastName LIKE @LastName
                         ORDER BY s.LastName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LastName", "%" + lastName + "%");

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Student student = new Student
                            {
                                StudentID = Convert.ToInt32(reader["StudentID"]),
                                StudentNumber = reader["StudentNumber"].ToString() ?? "",
                                FirstName = reader["FirstName"].ToString() ?? "",
                                LastName = reader["LastName"].ToString() ?? "",
                                PhoneNumber = reader["PhoneNumber"].ToString() ?? "",
                                ClassName = reader["Class"].ToString() ?? "",
                                RoomID = reader["RoomID"] != DBNull.Value ? Convert.ToInt32(reader["RoomID"]) : (int?)null,
                                Room = new Room
                                {
                                    RoomID = reader["RoomID"] != DBNull.Value ? Convert.ToInt32(reader["RoomID"]) : 0,
                                    RoomNumber = reader["RoomNumber"].ToString() ?? ""
                                }
                            };

                            students.Add(student);
                        }
                    }
                }
            }

            return students;
        }
    }
}
