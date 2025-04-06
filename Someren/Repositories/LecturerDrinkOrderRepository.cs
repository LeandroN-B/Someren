using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories
{
    public class LecturerDrinkOrderRepository : ILecturerDrinkOrderRepository
    {
        private readonly string connectionString;

        public LecturerDrinkOrderRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void AddLecturerDrinkOrder(LecturerDrinkOrder order)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "INSERT INTO LecturerDrinkOrders (LecturerId, DrinkId, Quantity) VALUES (@lecturerId, @drinkId, @quantity)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@lecturerId", order.LecturerId);
                    cmd.Parameters.AddWithValue("@drinkId", order.DrinkId);
                    cmd.Parameters.AddWithValue("@quantity", order.Quantity);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
