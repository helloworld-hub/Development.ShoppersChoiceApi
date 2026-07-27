
using ShoppersChoice.Entities.DTOs;

namespace ShoppersChoice.Entities
{
    public class CreateOrderResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public OrderResponseDto? Order { get; set; }
    }
}
