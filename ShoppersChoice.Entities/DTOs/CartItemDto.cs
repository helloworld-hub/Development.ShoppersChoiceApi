using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppersChoice.Entities.DTOs
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public List<string> ProductImage { get; set; }
        public double? Price { get; set; }
        public int Quantity { get; set; }
        public double? Subtotal { get; set; }
    }
}
