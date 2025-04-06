using Someren.Models;

namespace Someren.Repositories
{
    public interface IDrinkRepository
    {
        List<Drink> GetAllDrinks();
        void ResetAllStocks();
        void ReduceStock(int drinkId, int quantity);
        void IncrementDrinkOrderCount(int drinkId, string userType, int quantity);
    }
}
