using ShoppersChoice.API.Models;
using ShoppersChoice.Entities;
using ShoppersChoice.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppersChoice.DataAccess.Contracts
{
    public interface IOrderRepos
    {
      Task<List<OrderResponseDto>> GetOrders();

      Task<CreateOrderResult> CreateOrderAsync(CreateOrderDto dto);
    }
}
