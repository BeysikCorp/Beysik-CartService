using Microsoft.AspNetCore.Mvc;
using Beysik_CartService.Models;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Beysik_CartService.Services

{
        public class CartService
        {
        private readonly ISQLiteConnection _db;
        private readonly List<CartItem> _cartItems = new();

        public CartService(ISQLiteConnection SQLiteConnection)
        {
            _db = SQLiteConnection;
            _db.CreateTable<Cart>();
        }

        public void AddCart(Cart cart)
        {
            _db.Insert(cart);
        }

        public Cart GetItemById(int id)
        {
            return _db.Table<Cart>().FirstOrDefault(q => q.Id == id);
        }

        public void UpdateItemById(Cart cart)
        {
            var existingCart = GetItemById(cart.Id);
            if (existingCart == null)
                throw new Exception($"Cart with id {cart.Id} not found.");
            _db.Update(cart);
        }

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