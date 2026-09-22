using Basket.Core.Entities;

namespace Basket.Core.Repositories
{
    public interface IBasketRepository
    {
        Task<ShoppingCart> GetBasketAsync(string userName);
        Task<ShoppingCart> UpsertBasketAsync(ShoppingCart shoppingCart);
        Task DeleteBasketAsync(string userName);
    }
}
