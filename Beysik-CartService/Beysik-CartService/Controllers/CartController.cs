using Beysik_CartService.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Beysik_CartService.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Beysik_CartService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("/items")]
        public async Task<IActionResult> GetCart([FromQuery] string userId)
        {
            var cart = _cartService.GetCart(userId);
            return Ok(cart);
        }


        [HttpPost("/items")]
        public async Task<IActionResult> AddItem([FromQuery] string userId, [FromBody] CartItem newItem)
        {
            await _cartService.AddItem(userId, newItem);
            return CreatedAtAction(nameof(GetCart), new { userId = userId }, newItem);
        }

        [HttpPut("/items")]
        public async Task<IActionResult> UpdateItem([FromQuery] string userId, [FromBody] CartItem updatedItem)
        {
            await _cartService.UpdateCart(userId, updatedItem);
            return Ok();
        }

        [HttpDelete("/items")]
        public async Task<IActionResult> DeleteItem([FromQuery] string userId, [FromQuery, Optional] string productId, [FromQuery, Optional] bool all)
        {
            await _cartService.RemoveItem(userId, productId, all);
            return Ok();
        }
    }

}