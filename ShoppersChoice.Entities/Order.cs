using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppersChoice.API.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } // Pending, Processing, Shipped, Delivered, Cancelled

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public DateTime? DeliveryDate { get; set; }

        [StringLength(500)]
        public string ShippingAddress { get; set; }
        public User User { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}