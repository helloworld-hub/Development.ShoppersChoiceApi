

namespace ShoppersChoice.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }

        public double? price { get; set; }

        public int? discountPercent { get; set; }

        public List<string>? images { get; set; }

        public string? sizes { get; set;}

        public string? category { get; set; }

        public string? sellerId { get; set; }

        public string? createdAt { get; set; }

        public int? stock {  get; set; }

        public string? thumbnails { get; set; }

    }
}
