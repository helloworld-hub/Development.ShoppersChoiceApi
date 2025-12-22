public class ProductUpdateDto
{
    public string? name { get; set; }
    public double? price { get; set; }
    public int? discountPercent { get; set; }
    public string? category { get; set; }
    public List<string>? images { get; set; }
    public int? stock { get; set; }
}
