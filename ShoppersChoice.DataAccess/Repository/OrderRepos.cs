using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShoppersChoice.API.Models;
using ShoppersChoice.DataAccess.Contracts;
using ShoppersChoice.DataAccess.Migrations;
using ShoppersChoice.Entities.DTOs;
using ShoppersChoiceSevice.MySQLDbContext;
using System.Security.Claims;
using ShoppersChoice.Entities;

namespace ShoppersChoice.DataAccess.Repository
{
    public class OrderRepos : IOrderRepos
    {
        private readonly MySQLDBContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor; 
        public OrderRepos(MySQLDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<OrderResponseDto>> GetOrders()
        {
            int userId = GetUserId();

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(MapToDto).ToList();
        }

        public async Task<CreateOrderResult> CreateOrderAsync(CreateOrderDto dto)
        {
            int userId = GetUserId();

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return new CreateOrderResult
                {
                    Success = false,
                    Message = "Cart is empty"
                };
            }

            foreach (var item in cart.CartItems)
            {
                if (item.Product == null)
                {
                    return new CreateOrderResult
                    {
                        Success = false,
                        Message = $"Product {item.ProductId} no longer exists"
                    };
                }

                if (item.Product.stock.HasValue && item.Product.stock.Value < item.Quantity)
                    return new CreateOrderResult
                    {
                        Success = false,
                        Message = $"Insufficient stock for '{item.Product.name}'"
                    };
            }

            var order = MapOrder(dto, cart);
            _context.Orders.Add(order);

            // Decrement stock for each purchased product
            foreach (var item in cart.CartItems)
            {
                if (item.Product.stock.HasValue)
                {
                    item.Product.stock = item.Product.stock.Value - item.Quantity;
                }
            }

            // Empty the cart now that the order has been placed
            _context.CartItems.RemoveRange(cart.CartItems);
            cart.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var createdOrder = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            return new CreateOrderResult
            {
                Success = true,
                Message = "Order created successfully.",
                Order = MapToDto(createdOrder)
            };
        }

        #region Helper Methods

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

        private static Order MapOrder(CreateOrderDto order, Cart cart)
        {
            return new Order
            {
                UserId = cart.UserId,
                OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{cart.UserId}",
                Status = "Pending",
                OrderDate = DateTime.Now,
                ShippingAddress = order.ShippingAddress,
                TotalAmount = (decimal)cart.CartItems.Sum(ci => ci.Subtotal ?? 0),
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = (decimal)(ci.Price ?? 0),
                    Subtotal = (decimal)(ci.Subtotal ?? 0)
                }).ToList()
            };
        }

        // Helper method to get current user ID 
        private int GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var idValue = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idValue, out var id))
                return id;

            throw new InvalidOperationException("Authenticated user ID claim not found.");
        }

        #endregion
    }
}
