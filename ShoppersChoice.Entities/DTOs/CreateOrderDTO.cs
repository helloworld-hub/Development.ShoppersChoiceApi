
using System.ComponentModel.DataAnnotations;

namespace ShoppersChoice.Entities.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; }
    }
}
