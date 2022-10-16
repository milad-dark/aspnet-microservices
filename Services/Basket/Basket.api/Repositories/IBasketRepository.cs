using Basket.api.Entities;

namespace Basket.api.Repositories
{
    public interface IBasketRepository
    {
        Task<ShoppingCart> GetUserBasket(string username);
        Task<ShoppingCart> UpdateBasket(ShoppingCart basket);
        Task DeleteBasket(string username);
    }
}
