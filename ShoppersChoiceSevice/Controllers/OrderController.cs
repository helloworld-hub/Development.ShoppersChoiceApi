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

    }
}