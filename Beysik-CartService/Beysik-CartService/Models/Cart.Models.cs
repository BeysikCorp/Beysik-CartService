using Microsoft.AspNetCore.Mvc;
namespace Beysik_CartService.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();

    }
}
