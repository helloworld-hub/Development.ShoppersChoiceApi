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
    [Route("api/orders")]
    [Authorize]  // All endpoints require authentication
    public class OrderController : ControllerBase
    {
        private static readonly string[] ValidStatuses =
        {
            "Pending", "Processing", "Shipped", "Delivered", "Cancelled"
        };

        private readonly MySQLDBContext _context;

        public OrderController(MySQLDBContext context)
        {
            _context = context;
        }

        // Helper method to get current user ID
        private int GetUserId()
        {
            return int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        }

        private static OrderResponseDto MapToDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliveryDate,
                ShippingAddress = order.ShippingAddress,
                Items = order.OrderItems?.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product != null ? oi.Product.name : null,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    Subtotal = oi.Subtotal
                }).ToList()
            };
        }
        // GET: api/orders
        // Returns the current user's order history
        [HttpGet]
        public async Task<ActionResult> GetOrders()
        {
            try
            {
                int userId = GetUserId();

                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                return Ok(orders.Select(MapToDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
       
    }
}