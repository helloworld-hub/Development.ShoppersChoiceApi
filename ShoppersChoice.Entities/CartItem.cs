using ShoppersChoice.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppersChoice.API.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public double? Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public double? Subtotal { get; set; }

        public  Cart Cart { get; set; }

        public  Product Product { get; set; }
    }
}