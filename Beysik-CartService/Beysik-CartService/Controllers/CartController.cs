using Beysik_CartService.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Beysik_CartService.Models;
namespace Beysik_CartService.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(string userId)
        {
            var cart = await _cartService.GetAsync(userId);
            return Ok(cart);
        }
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] CartItem newItem)
        {
            await _cartService.AddAsync(newItem);
            return CreatedAtAction(nameof(GetCart), new { userId = newItem.Id }, newItem);
        }

        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] CartItem updatedItem)
        {
            await _cartService.UpdateAsync(id.ToString(), updatedItem);
            return NoContent();
        }

        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _cartService.RemoveAsync(id.ToString());
            return NoContent();
        }
    }

}