using System;
using System.Collections.Generic;

namespace ShoppersChoice.Entities.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}