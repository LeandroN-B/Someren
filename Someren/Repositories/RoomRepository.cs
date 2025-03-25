using Microsoft.Data.SqlClient;
using Someren.Models;
using System;

namespace Someren.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly string _connectionString;

        // Constructor to initialize connection string
        public RoomRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("test1database")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        // Method for retrieving all rooms
        public List<Room> GetAllRooms()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT RoomID, RoomNumber, RoomType, Capacity, Floor, Building FROM Room ORDER BY RoomNumber";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return ReadRoomsFromReader(reader);
                    }
                }
            }
        }

        // Method for retrieving a room by its ID
        public Room? GetRoomByID(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT RoomID, RoomNumber, RoomType, Capacity, Floor, Building " +
                               "FROM Room " +
                               "WHERE RoomID = @RoomID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", id);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Room> rooms = ReadRoomsFromReader(reader);
                        return rooms.FirstOrDefault();
                    }
                }
            }
        }

        // Method to load the lecturer or students into a single room object
        public void LoadOccupantsForRoom(Room room)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                if (room.RoomType == RoomType.Single)
                {
                    string query = "SELECT LecturerID, FirstName AS LecturerFirstName, LastName AS LecturerLastName, PhoneNumber AS LecturerPhone, DateOfBirth, LecInDate, LecOutDate " +
                                   "FROM Lecturer " +
                                   "WHERE RoomID = @RoomID";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoomID", room.RoomID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                room.Lecturer = ReadLecturer(reader, room.RoomID);
                            }
                        }
                    }
                }
                else if (room.RoomType == RoomType.Dormitory)
                {
                    string query = @"SELECT StudentID, StudentNumber, FirstName AS StudentFirstName, LastName AS StudentLastName, PhoneNumber AS StudentPhone, [Class] AS StudentClass 
                                     FROM Student 
                                     WHERE RoomID = @RoomID";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoomID", room.RoomID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            room.Students = new List<Student>();
                            while (reader.Read())
                            {
                                room.Students.Add(ReadStudent(reader, room.RoomID));
                            }
                        }
                    }
                }
            }
        }


        // Method for adding a room with validation (checks duplicate room numbers)
        public void AddRoomIfNotExists(Room room)
        {
            if (room.RoomType == RoomType.Single && room.Capacity > 1)
                throw new ArgumentException("Single rooms cannot have more than 1 person.");

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Check for duplicate room number
                string checkQuery = "SELECT RoomID " +
                                    "FROM Room " +
                                    "WHERE RoomNumber = @RoomNumber";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                    using (SqlDataReader reader = checkCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                            throw new InvalidOperationException($"Room number '{room.RoomNumber}' already exists.");
                    }
                }

                // Insert the new room if validation passed
                string insertQuery = "INSERT INTO Room (RoomNumber, RoomType, Capacity, Floor, Building) VALUES (@RoomNumber, @RoomType, @Capacity, @Floor, @Building)";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    AddRoomParameters(insertCommand, room);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        // Method for updating existing room
        public void UpdateRoom(Room room)
        {
            if (room.RoomType == RoomType.Single && room.Capacity > 1)
                throw new ArgumentException("Single rooms cannot have more than 1 person.");

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Room SET RoomNumber = @RoomNumber, RoomType = @RoomType, Capacity = @Capacity, Floor = @Floor, Building = @Building WHERE RoomID = @RoomID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", room.RoomID);
                    AddRoomParameters(command, room);

                    connection.Open();
                    if (command.ExecuteNonQuery() == 0)
                        throw new Exception("No records updated!");
                }
            }
        }

        // Method for deleting a room
        public void DeleteRoom(Room room)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Room WHERE RoomID = @RoomID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", room.RoomID);
                    connection.Open();
                    if (command.ExecuteNonQuery() == 0)
                        throw new Exception("No records deleted!");
                }
            }
        }

        // Method for retrieving rooms along with associated lecturers and students
        public List<Room> GetRoomsWithPeople(int capacity = 0)
        {
            List<Room> rooms = new List<Room>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Step 1: Fetch rooms
                string queryRooms = "SELECT RoomID, RoomNumber, RoomType, Capacity, Floor, Building FROM Room";
                if (capacity == 1 || capacity == 8)
                {
                    queryRooms += " WHERE Capacity = @Capacity";
                }
                queryRooms += " ORDER BY RoomNumber";

                using (SqlCommand cmdRooms = new SqlCommand(queryRooms, connection))
                {
                    if (capacity == 1 || capacity == 8)
                    {
                        cmdRooms.Parameters.AddWithValue("@Capacity", capacity);
                    }

                    connection.Open();
                    using (SqlDataReader readerRooms = cmdRooms.ExecuteReader())
                    {
                        rooms = ReadRoomsFromReader(readerRooms);
                    }
                    connection.Close();
                }

                // Step 2: For each room, fetch lecturer and students
                foreach (Room room in rooms)
                {
                    // Fetch lecturer
                    string queryLecturer = "SELECT LecturerID, FirstName AS LecturerFirstName, LastName AS LecturerLastName, PhoneNumber AS LecturerPhone, DateOfBirth, LecInDate, LecOutDate" +
                                           "FROM Lecturer " +
                                           "WHERE RoomID = @RoomID";

                    using (SqlCommand cmdLecturer = new SqlCommand(queryLecturer, connection))
                    {
                        cmdLecturer.Parameters.AddWithValue("@RoomID", room.RoomID);
                        connection.Open();
                        using (SqlDataReader readerLecturer = cmdLecturer.ExecuteReader())
                        {
                            if (readerLecturer.Read())
                            {
                                room.Lecturer = ReadLecturer(readerLecturer, room.RoomID);
                            }
                        }
                        connection.Close();
                    }

                    // Fetch students
                    string queryStudents = @"SELECT StudentID, StudentNumber, FirstName AS StudentFirstName, LastName AS StudentLastName, PhoneNumber AS StudentPhone, [Class] AS StudentClass 
                                            FROM Student 
                                            WHERE RoomID = @RoomID";

                    using (SqlCommand cmdStudents = new SqlCommand(queryStudents, connection))
                    {
                        cmdStudents.Parameters.AddWithValue("@RoomID", room.RoomID);
                        connection.Open();
                        using (SqlDataReader readerStudents = cmdStudents.ExecuteReader())
                        {
                            room.Students = new List<Student>();
                            while (readerStudents.Read())
                            {
                                room.Students.Add(ReadStudent(readerStudents, room.RoomID));
                            }
                        }
                        connection.Close();
                    }
                }
            }

            return rooms;
        }


        // --------------------------------------
        // PRIVATE METHODS
        // --------------------------------------

        // Adds parameters for inserting/updating a room
        private void AddRoomParameters(SqlCommand command, Room room)
        {
            command.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
            command.Parameters.AddWithValue("@RoomType", room.RoomType.ToString());
            command.Parameters.AddWithValue("@Capacity", room.Capacity);
            command.Parameters.AddWithValue("@Floor", room.Floor);
            command.Parameters.AddWithValue("@Building", room.Building.ToString());
        }

        // Method for reading multiple rooms from SqlDataReader
        private List<Room> ReadRoomsFromReader(SqlDataReader reader)
        {
            List<Room> rooms = new List<Room>();  // explicitly declared List<Room>
            while (reader.Read())
                rooms.Add(ReadSingleRoom(reader));
            return rooms;
        }


        // Method for reading a single room from SqlDataReader
        private Room ReadSingleRoom(SqlDataReader reader)
        {
            return new Room
            {
                RoomID = reader.GetInt32(reader.GetOrdinal("RoomID")),
                RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber")),
                RoomType = Enum.TryParse(reader.GetString(reader.GetOrdinal("RoomType")), out RoomType roomType)
                    ? roomType : RoomType.Dormitory,
                Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),
                Floor = reader.GetInt32(reader.GetOrdinal("Floor")),
                Building = Enum.TryParse(reader.GetString(reader.GetOrdinal("Building")), out BuildingType buildingType)
                    ? buildingType : BuildingType.A,
                Lecturer = null,   // Occupants will be assigned separately
                Students = new List<Student>()
            };
        }

        // Method for reading lecturer data from SqlDataReader
        private Lecturer ReadLecturer(SqlDataReader reader, int roomId)
        {
            
            return new Lecturer
            {
                LecturerID = reader.GetInt32(reader.GetOrdinal("LecturerID")),
                FirstName = reader.GetString(reader.GetOrdinal("LecturerFirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LecturerLastName")),
                PhoneNumber = reader.GetString(reader.GetOrdinal("LecturerPhone")),
                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                LecInDate = reader.IsDBNull(reader.GetOrdinal("LecInDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LecInDate")),
                LecOutDate = reader.IsDBNull(reader.GetOrdinal("LecOutDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LecOutDate")),

                RoomID = roomId
            };
        }


        // Method for reading student data from SqlDataReader
        private Student ReadStudent(SqlDataReader reader, int roomId)
        {
            return new Student
            {
                StudentID = reader.GetInt32(reader.GetOrdinal("StudentID")),
                StudentNumber = reader.GetInt32(reader.GetOrdinal("StudentNumber")).ToString(),
                FirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                LastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                PhoneNumber = reader.GetString(reader.GetOrdinal("StudentPhone")),
                ClassName = reader.IsDBNull(reader.GetOrdinal("StudentClass"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("StudentClass")).ToString() ?? string.Empty,
                RoomID = roomId
            };
        }

    }
}