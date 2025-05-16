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

        public CartController(CartService cartService) =>
            _cartService = cartService;

        [HttpGet("{userId}")]
        public async Task<ActionResult<Cart>> GetCart(string userId)
        {
            var cart = await _cartService.GetAsync(userId);
            return Ok(cart);
        }

        [HttpPost("items/?user={userId}")]
        public async Task<IActionResult> AddItem([FromQuery] string userId, [FromBody] CartItem newItem)
        {
            await _cartService.AddAsync(newItem);
            return CreatedAtAction(nameof(GetCart), new { userId = userId }, newItem);
        }

        [HttpPut("items?user={userId}")]
        public async Task<IActionResult> UpdateItem([FromQuery]string userId, [FromBody] CartItem updatedItem)
        {
            var success = await _cartService.UpdateAsync(userId, updatedItem);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("/items/{productId}?user={userId}")]
        public async Task<IActionResult> DeleteItem(string userId, string productId)
        {
            var success = await _cartService.RemoveItemAsync(userId, productId);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            await _cartService.ClearCartAsync(userId);
            return Ok("Cart cleared.");
        }

    }
        
}
