using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SQLite;
namespace Beysik_CartService.Models
{
    public class CartItem
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
    public class Cart
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ItemsJson { get; set; }

        [Ignore]
        public List<CartItem> Items
        {
            get => string.IsNullOrEmpty(ItemsJson) ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(ItemsJson);
            set => ItemsJson = JsonSerializer.Serialize(value);
        }
    }
}