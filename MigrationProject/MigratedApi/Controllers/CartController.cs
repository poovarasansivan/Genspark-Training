using MigratedApi.Models.Dtos;
using MigratedApi.Interfaces;
using MigratedApi.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MigratedApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] CartRequestDto cartDto)
        {
            try
            {
                var createdCart = await _cartService.AddToCartAsync(cartDto);
                return Ok(SuccessResponseHandler.Success(createdCart, "Item added to cart successfully."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCarts()
        {
            try
            {
                var carts = await _cartService.GetAllCartsAsync();
                return Ok(SuccessResponseHandler.Success(carts, "Carts retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetCartsByUserId(int userId)
        {
            try
            {
                var carts = await _cartService.GetCartByIdAsync(userId);
                return Ok(SuccessResponseHandler.Success(carts, "Carts for user retrieved successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCartById(int id)
        {
            try
            {
                var cart = await _cartService.GetCartByIdAsync(id);
                return Ok(SuccessResponseHandler.Success(cart, "Cart retrieved successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCart(int id, [FromBody] CartRequestDto cartDto)
        {
            try
            {
                var updatedCart = await _cartService.UpdateCartAsync(id, cartDto);
                return Ok(SuccessResponseHandler.Success(updatedCart, "Cart updated successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            try
            {
                var result = await _cartService.RemoveFromCartAsync(id);
                return Ok(SuccessResponseHandler.Success(result, "Item removed from cart successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("total/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetTotalPrice(int userId)
        {
            try
            {
                var totalPrice = await _cartService.GetTotalPriceAsync(userId);
                return Ok(SuccessResponseHandler.Success(totalPrice, "Total price retrieved successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}