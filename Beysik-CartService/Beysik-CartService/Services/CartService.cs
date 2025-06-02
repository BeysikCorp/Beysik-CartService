using Microsoft.AspNetCore.Mvc;
using Beysik_CartService.Models;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beysik_Common;
using static Beysik_Common.RabbitMqConsumerService;
using RabbitMQ.Client;

namespace Beysik_CartService.Services

{
    public class CartService
    {
        private readonly ISQLiteConnection _db;
        //private readonly List<CartItem> _cartItems = new();
        private readonly RabbitMqHelper _rabbitMqHelper;

        public CartService(RabbitMqEventAggregator eventAggregator, ISQLiteConnection SQLiteConnection, RabbitMqHelper rabbitMqHelper)
        {
            eventAggregator.MessageReceived += OnMessageReceived;
            _db = SQLiteConnection;
            _db.CreateTable<Cart>();

            _rabbitMqHelper = rabbitMqHelper;
        }
        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e == null || string.IsNullOrEmpty(e.Message))
            {
                return;
            }

            List<string>? message = e.Message.Split('.').ToList();

            if (e.Message.Contains("order.allocated"))
            {
                int orderId = int.Parse(message[0]);
                int quantity = int.Parse(message[1]);
                
            }
            //Console.WriteLine($"Message received: {e.Message}");
        }

        public void AddCart(string userId)
        {
            _db.Insert(new Cart
            {
                UserId = userId,
                ItemsJson = string.Empty,
                Items = new List<CartItem>()
            });
        }

        public Cart GetCart(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));
            var cart = _db.Table<Cart>().FirstOrDefault(q => q.UserId == userId);
            do
            {
                cart = _db.Table<Cart>().FirstOrDefault(q => q.UserId == userId);
                if (cart != null)
                {
                    try
                    {
                        cart.Items = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cart.ItemsJson) ?? new List<CartItem>();
                    }
                    catch
                    {
                        cart.Items = new List<CartItem>();
                    }
                }
                else
                {
                    AddCart(userId);
                }
            }
            while (cart == null);
            return cart;
        }

        public Task AddItem(string userId, CartItem newItem)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));
            var cart = GetCart(userId);
            if (cart == null)
                cart.Items = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cart.ItemsJson) ?? new List<CartItem>();

            // Prevent duplicate ProductId
            if (cart.Items.Any(item => item.ProductId == newItem.ProductId))
            {
                _rabbitMqHelper.PublishMessage($"{cart.Items}.cart.itemduplicate", "cart.toui",
                    "cart.api.fromcart", ExchangeType.Topic);
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = newItem.ProductId,
                    Name = newItem.Name,
                    Price = newItem.Price,
                    Quantity = newItem.Quantity
                });

                UpdateCart(cart.UserId, updatedItem: newItem);
                _rabbitMqHelper.PublishMessage($"{newItem.ProductId}.cart.itemadded", "cart.toui","cart.api.fromcart", ExchangeType.Topic);
            }
            return Task.CompletedTask;
        }

        public Task UpdateCart(string userId, CartItem updatedItem)
        {
            var cart = GetCart(userId);
            if (cart == null)
                throw new KeyNotFoundException($"Cart for user {userId} not found.");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));
            if (updatedItem == null)
                throw new ArgumentNullException(nameof(updatedItem));

            var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == updatedItem.ProductId);

            if (existingItem == null)
            {
                throw new KeyNotFoundException($"CartItem with ProductId {updatedItem.ProductId} not found.");
            }

            existingItem = updatedItem;
            if (cart.Items != null)
            {
                cart.ItemsJson = System.Text.Json.JsonSerializer.Serialize(cart.Items);
            }
            _db.Update(cart);
            return Task.CompletedTask;
        }


        public Task RemoveItem(string productId, string userId, bool? all)
        {
            if(all == null)
                all = false;
            var cart = GetCart(userId);
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentNullException(nameof(productId));

            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                if (all.Value == false)
                    cart.Items = new List<CartItem>();
                else
                    cart.Items.Remove(item);
                if (cart.Items != null)
                {
                    cart.ItemsJson = System.Text.Json.JsonSerializer.Serialize(cart.Items);
                }
                _db.Update(cart);

            }
            else
            {
                throw new KeyNotFoundException($"CartItem with ProductId {productId} not found.");
            }
            return Task.CompletedTask;
        }
    }
}