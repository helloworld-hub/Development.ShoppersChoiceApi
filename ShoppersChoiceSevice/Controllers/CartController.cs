using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppersChoice.API.Models;
using ShoppersChoice.Entities.DTOs;
using ShoppersChoiceSevice.MySQLDbContext;

namespace ShoppersChoice.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]  // All endpoints require authentication
    public class CartController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public CartController(MySQLDBContext context)
        {
            _context = context;
        }

        // Helper method to get current user ID
        private int GetUserId()
        {
            return int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        }

        // GET: api/cart
        [HttpGet]
        public async Task<ActionResult<CartResponseDto>> GetCart()
        {
            try
            {
                int userId = GetUserId();

                // Get or create cart for user
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    // Create new cart if doesn't exist
                    cart = new Cart
                    {
                        UserId = userId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                // Build response
                var response = new CartResponseDto
                {
                    Id = cart.Id,
                    UserId = cart.UserId,
                    Items = cart.CartItems.Select(ci => new CartItemDto
                    {
                        Id = ci.Id,
                        ProductId = ci.ProductId,
                        ProductName = ci.Product.name,
                        ProductImage =  ci.Product.images,
                        Price = ci.Price,
                        Quantity = ci.Quantity,
                        Subtotal = ci.Subtotal
                    }).ToList(),
                    TotalItems = cart.CartItems.Sum(ci => ci.Quantity),
                    TotalPrice = cart.CartItems.Sum(ci => ci.Subtotal)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // POST: api/cart/add
        [HttpPost("add")]
        public async Task<ActionResult<CartResponseDto>> AddToCart(AddToCartDto dto)
        {
            try
            {
                int userId = GetUserId();

                // Get or create cart
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                // Get product
                var product = await _context.Products.FindAsync(dto.ProductId);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }

                // Check if product already in cart
                var existingItem = cart.CartItems
                    .FirstOrDefault(ci => ci.ProductId == dto.ProductId);

                if (existingItem != null)
                {
                    // Update quantity
                    existingItem.Quantity += dto.Quantity;
                    existingItem.Subtotal = existingItem.Quantity * existingItem.Price;
                }
                else
                {
                    // Add new item
                    var cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity,
                        Price = product.price,
                        Subtotal = product.price * dto.Quantity
                    };
                    _context.CartItems.Add(cartItem);
                }

                cart.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                // Return updated cart
                return await GetCart();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // PUT: api/cart/update
        [HttpPut("update")]
        public async Task<ActionResult<CartResponseDto>> UpdateCartItem(UpdateCartItemDto dto)
        {
            try
            {
                int userId = GetUserId();

                var cartItem = await _context.CartItems
                    .Include(ci => ci.Cart)
                    .FirstOrDefaultAsync(ci => ci.Id == dto.CartItemId && ci.Cart.UserId == userId);

                if (cartItem == null)
                {
                    return NotFound(new { message = "Cart item not found" });
                }

                // Update quantity
                cartItem.Quantity = dto.Quantity;
                cartItem.Subtotal = cartItem.Price * dto.Quantity;
                cartItem.Cart.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return await GetCart();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // DELETE: api/cart/remove/{cartItemId}
        [HttpDelete("remove/{cartItemId}")]
        public async Task<ActionResult<CartResponseDto>> RemoveFromCart(int cartItemId)
        {
            try
            {
                int userId = GetUserId();

                var cartItem = await _context.CartItems
                    .Include(ci => ci.Cart)
                    .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

                if (cartItem == null)
                {
                    return NotFound(new { message = "Cart item not found" });
                }

                _context.CartItems.Remove(cartItem);
                cartItem.Cart.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return await GetCart();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // DELETE: api/cart/clear
        [HttpDelete("clear")]
        public async Task<ActionResult> ClearCart()
        {
            try
            {
                int userId = GetUserId();

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    return NotFound(new { message = "Cart not found" });
                }

                _context.CartItems.RemoveRange(cart.CartItems);
                cart.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Cart cleared successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}