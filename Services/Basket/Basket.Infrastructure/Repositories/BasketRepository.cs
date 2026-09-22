using Basket.Core.Entities;
using Basket.Core.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.Infrastructure.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _cache;

        public BasketRepository(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task DeleteBasketAsync(string userName)
        {
            await _cache.RemoveAsync(userName);
        }

        public async Task<ShoppingCart> GetBasketAsync(string userName)
        {
            var basket = await _cache.GetStringAsync(userName);
            if(string.IsNullOrEmpty(basket))
            {
                return new ShoppingCart(userName);
            }
            return JsonSerializer.Deserialize<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpsertBasketAsync(ShoppingCart shoppingCart)
        {
            var basket = JsonSerializer.Serialize(shoppingCart);
            await _cache.SetStringAsync(shoppingCart.UserName, basket);
            return await GetBasketAsync(shoppingCart.UserName);
        }
    }
}
