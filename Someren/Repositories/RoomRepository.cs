using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly string _connectionString;

        public RoomRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("LMBdatabase")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public List<Room> GetAllRooms()
        {
            List<Room> rooms = new List<Room>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT RoomID, RoomNumber, RoomType, Capacity, Floor, Building FROM Rooms";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Extract RoomType as a char (first character of string)
                            string roomTypeStr = reader["RoomType"].ToString() ?? string.Empty;
                            char roomTypeChar = string.IsNullOrEmpty(roomTypeStr) ? default(char) : roomTypeStr[0];
                            // Get Building as string directly
                            string buildingValue = reader["Building"].ToString() ?? "Single";

                            rooms.Add(new Room
                            {
                                RoomID = Convert.ToInt32(reader["RoomID"]),
                                RoomNumber = reader["RoomNumber"].ToString() ?? string.Empty,
                                RoomType = roomTypeChar,
                                Capacity = Convert.ToInt32(reader["Capacity"]),
                                Floor = Convert.ToInt32(reader["Floor"]),
                                Building = buildingValue
                            });
                        }
                    }
                }
            }
            return rooms;
        }

        public Room? GetRoomByID(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT RoomID, RoomNumber, RoomType, Capacity, Floor, Building FROM Rooms WHERE RoomID = @RoomID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", id);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string roomTypeStr = reader["RoomType"].ToString() ?? string.Empty;
                            char roomTypeChar = string.IsNullOrEmpty(roomTypeStr) ? default(char) : roomTypeStr[0];
                            string buildingValue = reader["Building"].ToString() ?? "Single";

                            return new Room
                            {
                                RoomID = Convert.ToInt32(reader["RoomID"]),
                                RoomNumber = reader["RoomNumber"].ToString() ?? string.Empty,
                                RoomType = roomTypeChar,
                                Capacity = Convert.ToInt32(reader["Capacity"]),
                                Floor = Convert.ToInt32(reader["Floor"]),
                                Building = buildingValue
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void AddRoom(Room room)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Rooms (RoomNumber, RoomType, Capacity, Floor, Building) VALUES (@RoomNumber, @RoomType, @Capacity, @Floor, @Building)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                    // Convert RoomType char to string for saving.
                    command.Parameters.AddWithValue("@RoomType", room.RoomType.ToString());
                    command.Parameters.AddWithValue("@Capacity", room.Capacity);
                    command.Parameters.AddWithValue("@Floor", room.Floor);
                    command.Parameters.AddWithValue("@Building", room.Building);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateRoom(Room room)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Rooms SET RoomNumber = @RoomNumber, RoomType = @RoomType, Capacity = @Capacity, Floor = @Floor, Building = @Building WHERE RoomID = @RoomID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", room.RoomID);
                    command.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                    command.Parameters.AddWithValue("@RoomType", room.RoomType.ToString());
                    command.Parameters.AddWithValue("@Capacity", room.Capacity);
                    command.Parameters.AddWithValue("@Floor", room.Floor);
                    command.Parameters.AddWithValue("@Building", room.Building);

                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows == 0)
                    {
                        throw new Exception("No records updated!");
                    }
                }
            }
        }

        public void DeleteRoom(Room room)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Rooms WHERE RoomID = @RoomID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", room.RoomID);
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
