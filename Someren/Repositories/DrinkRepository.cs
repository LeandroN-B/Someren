using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories
{
    public class DrinkRepository : IDrinkRepository
    {
        private readonly string connectionString;

        public DrinkRepository(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public List<Drink> GetAllDrinks()
        {
            var drinks = new List<Drink>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT DrinkId, Name, Stock, Price, VAT, DefaultStock, IsAlcoholic FROM Drink";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        drinks.Add(new Drink
                        {
                            DrinkId = (int)reader["DrinkId"],
                            Name = reader["Name"]!.ToString() ?? string.Empty,
                            Stock = (int)reader["Stock"],
                            Price = (decimal)reader["Price"],
                            VAT = (decimal)reader["VAT"],
                            DefaultStock = (int)reader["DefaultStock"],
                            IsAlcoholic = (bool)reader["IsAlcoholic"]
                        });
                    }
                }
            }

            return drinks;
        }

        public void ResetAllStocks()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Drink SET Stock = 120";
                new SqlCommand(query, conn).ExecuteNonQuery();
            }
        }

        public void ReduceStock(int drinkId, int quantity)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Drink SET Stock = Stock - @quantity WHERE DrinkId = @drinkId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@drinkId", drinkId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void IncrementDrinkOrderCount(int drinkId, string userType, int quantity)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string column = userType == "Student" ? "TotalOrderedByStudents" : "TotalOrderedByLecturers";
                string query = $"UPDATE Drink SET {column} = {column} + @quantity WHERE DrinkId = @drinkId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@drinkId", drinkId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
