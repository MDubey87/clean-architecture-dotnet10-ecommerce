using Basket.Application.Commands;
using Basket.Application.DTOs;
using Basket.Application.Responses;
using Basket.Core.Entities;

namespace Basket.Application.Mappers
{
    public static class BasketMapper
    {
        public static ShoppingCartResponse ToResponse(this ShoppingCart shoppingCart)
        {
            return new ShoppingCartResponse
            {
                UserName = shoppingCart.UserName,
                Items = shoppingCart.Items.Select(item => new ShoppingCartItemResponse
                {
                    Quantity = item.Quantity,
                    ImageFile = item.ImageFile,
                    Price = item.Price,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName
                }).ToList()
            };
        }
        //Delegate Based Mapper
        public static ShoppingCartResponse ToResponseUsingDelegate(this ShoppingCart cart)
            => MapCart(cart);

        public static readonly Func<ShoppingCart, ShoppingCartResponse> MapCart =
            cart => new ShoppingCartResponse
            {
                UserName = cart.UserName,
                Items = cart.Items.Select(item => new ShoppingCartItemResponse
                {
                    Quantity = item.Quantity,
                    ImageFile = item.ImageFile,
                    Price = item.Price,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName
                }).ToList()
            };
        public static ShoppingCart ToEntity(this CreateShoppingCartCommand command)
        {
            return new ShoppingCart
            {
                UserName = command.UserName,
                Items = command.Items.Select(item => new ShoppingCartItem
                {
                    Quantity = item.Quantity,
                    ImageFile = item.ImageFile,
                    Price = item.Price,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName
                }).ToList()
            };
        }
        public static ShoppingCartDto ToDto(this ShoppingCartResponse shoppingCart)
        {
            return new ShoppingCartDto
            (
                shoppingCart.UserName,
                shoppingCart.Items.Select(item =>
                    new ShoppingCartItemDto(
                        item.ProductId,
                        item.ProductName,
                        item.ImageFile,
                        item.Price,
                        item.Quantity
                    )
                ).ToList(),
                shoppingCart.TotalPrice
            );
        }
    }
}
