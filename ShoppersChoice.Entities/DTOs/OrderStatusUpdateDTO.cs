using System.ComponentModel.DataAnnotations;

namespace ShoppersChoice.Entities.DTOs
{
    public class OrderStatusUpdateDto
    {
        // Expected values: Pending, Processing, Shipped, Delivered, Cancelled
        [Required]
        [StringLength(20)]
        public string Status { get; set; }
    }
}