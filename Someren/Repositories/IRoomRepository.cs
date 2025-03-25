using Someren.Models;

namespace Someren.Repositories
{
    public interface IRoomRepository
    {
        // Retrieve all rooms from the database
        List<Room> GetAllRooms();

        // Retrieve all rooms including lecturers and students, optionally filtered by capacity
        List<Room> GetRoomsWithPeople(int capacity = 0);

        // Retrieve a specific room by ID
        Room? GetRoomByID(int id);

        // Add a new room to the database after validation (checks duplicates)
        void AddRoomIfNotExists(Room room);

        // Update an existing room in the database
        void UpdateRoom(Room room);

        // Delete a room from the database
        void DeleteRoom(Room room);

        void LoadOccupantsForRoom(Room room);
    }
}
