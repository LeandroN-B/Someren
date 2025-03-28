﻿using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly string _connectionString;

        public RoomRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("test1database")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public List<Room> GetAllRooms()
        {
            List<Room> rooms = new List<Room>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT RoomID, RoomNumber, RoomType, Capacity, Floor, Building FROM Room ORDER BY RoomNumber";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rooms.Add(new Room
                            {
                                RoomID = reader.GetInt32(0),
                                RoomNumber = reader.GetString(1),
                                RoomType = Enum.TryParse(reader.GetString(2), out RoomType roomType) ? roomType : RoomType.Dormitory,
                                Capacity = reader.GetInt32(3),
                                Floor = reader.GetInt32(4),
                                Building = Enum.TryParse(reader.GetString(5), out BuildingType buildingType) ? buildingType : BuildingType.A
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
                string query = "SELECT RoomID, RoomNumber, RoomType, Capacity, Floor, Building FROM Room WHERE RoomID = @RoomID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", id);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Room
                            {
                                RoomID = reader.GetInt32(0),
                                RoomNumber = reader.GetString(1),
                                RoomType = Enum.TryParse(reader.GetString(2), out RoomType roomType) ? roomType : RoomType.Dormitory,
                                Capacity = reader.GetInt32(3),
                                Floor = reader.GetInt32(4),
                                Building = Enum.TryParse(reader.GetString(5), out BuildingType buildingType) ? buildingType : BuildingType.A
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void AddRoom(Room room)
        {
            if (room.RoomType == RoomType.Single && room.Capacity > 1)
            {
                throw new ArgumentException("Single rooms cannot have more than 1 person.");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Room (RoomNumber, RoomType, Capacity, Floor, Building) VALUES (@RoomNumber, @RoomType, @Capacity, @Floor, @Building)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                    command.Parameters.AddWithValue("@RoomType", room.RoomType.ToString());
                    command.Parameters.AddWithValue("@Capacity", room.Capacity);
                    command.Parameters.AddWithValue("@Floor", room.Floor);
                    command.Parameters.AddWithValue("@Building", room.Building.ToString());

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateRoom(Room room)
        {
            if (room.RoomType == RoomType.Single && room.Capacity > 1)
            {
                throw new ArgumentException("Single rooms cannot have more than 1 person.");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Room SET RoomNumber = @RoomNumber, RoomType = @RoomType, Capacity = @Capacity, Floor = @Floor, Building = @Building WHERE RoomID = @RoomID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", room.RoomID);
                    command.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                    command.Parameters.AddWithValue("@RoomType", room.RoomType.ToString());
                    command.Parameters.AddWithValue("@Capacity", room.Capacity);
                    command.Parameters.AddWithValue("@Floor", room.Floor);
                    command.Parameters.AddWithValue("@Building", room.Building.ToString());

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
                string query = "DELETE FROM Room WHERE RoomID = @RoomID";
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
