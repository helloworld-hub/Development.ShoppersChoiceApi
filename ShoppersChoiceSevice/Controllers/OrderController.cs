using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppersChoice.API.Models;
using ShoppersChoice.DataAccess.Contracts;
using ShoppersChoice.DataAccess.Migrations;
using ShoppersChoice.Entities.DTOs;
using ShoppersChoiceSevice.MySQLDbContext;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        private readonly IOrderRepos _orderRepos;

        public OrderController(IOrderRepos orderRepos)
        {
            _orderRepos = orderRepos;
        }

     

        // Returns the current user's order history
        [HttpGet]
        public async Task<ActionResult> GetOrders()
        {
            try
            {
               var order = await _orderRepos.GetOrders();
              
               return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }


        // POST: api/orders
        // Creates an order (checkout) from the current user's cart
        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder(CreateOrderDto dto)
        {
            try
            {
                var result = await _orderRepos.CreateOrderAsync(dto);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(result.Order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // PUT: api/orders/{id}/cancel
        // Cancels an order that hasn't shipped yet
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var order = await _orderRepos.CancelOrder(id);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderRepos.GetOrderByIdAsync(id);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred",
                    error = ex.Message
                });
            }
        }
    }
}