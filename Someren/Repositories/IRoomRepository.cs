using Someren.Models;

namespace Someren.Repositories
{
    public interface IRoomRepository
    {
        List<Room> GetAllRooms();
        Room? GetRoomByID(int id);
        void AddRoom(Room room);
        void UpdateRoom(Room room);
        void DeleteRoom(Room room);

    }
}