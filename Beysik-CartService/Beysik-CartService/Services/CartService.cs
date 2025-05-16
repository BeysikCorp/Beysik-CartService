using Microsoft.AspNetCore.Mvc;
using Beysik_CartService.Models;
namespace Beysik_CartService.Services
{
    public class CartService
    {
        private readonly List<CartItem> _cartItems = new();

        public Task<List<CartItem>> GetAsync() =>
            Task.FromResult(_cartItems);

        public Task<CartItem?> GetAsync(string id) =>
            Task.FromResult(_cartItems.FirstOrDefault(item => item.ProductId == id));

        public Task AddAsync(CartItem newItem)
        {
            _cartItems.Add(newItem);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(string id, CartItem updatedItem)
        {
            var index = _cartItems.FindIndex(item => item.ProductId == id);
            if (index != -1)
            {
                _cartItems[index] = updatedItem;
            }
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string id)
        {
            var item = _cartItems.FirstOrDefault(x => x.ProductId == id);
            if (item != null)
            {
                _cartItems.Remove(item);
            }
            return Task.CompletedTask;
        }
    }
}