using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories
{
    public class DrinkOrderRepository : IDrinkOrderRepository
    {
        private readonly string connectionString;

        public DrinkOrderRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void AddDrinkOrder(DrinkOrder order)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check stock
                string checkStockQuery = "SELECT Stock FROM Drink WHERE DrinkId = @drinkId";
                using (SqlCommand checkCmd = new SqlCommand(checkStockQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@drinkId", order.DrinkId);
                    int currentStock = (int)checkCmd.ExecuteScalar()!;
                    if (order.Quantity > currentStock)
                    {
                        throw new InvalidOperationException("❌ Not enough stock available for this drink.");
                    }
                }

                // Insert drink order
                string insertQuery = "INSERT INTO DrinkOrders (StudentId, DrinkId, Quantity) VALUES (@studentId, @drinkId, @quantity)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@studentId", order.StudentId);
                    cmd.Parameters.AddWithValue("@drinkId", order.DrinkId);
                    cmd.Parameters.AddWithValue("@quantity", order.Quantity);
                    cmd.ExecuteNonQuery();
                }

                // Update stock
                string updateStockQuery = "UPDATE Drink SET Stock = Stock - @quantity WHERE DrinkId = @drinkId";
                using (SqlCommand updateCmd = new SqlCommand(updateStockQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@quantity", order.Quantity);
                    updateCmd.Parameters.AddWithValue("@drinkId", order.DrinkId);
                    updateCmd.ExecuteNonQuery();
                }
            }
        }
    }
}
