using ShoppersChoice.Entities;
using ShoppersChoice.Entities.DTOs;


namespace ShoppersChoice.DataAccess.NewFolder2
{
    public interface IProductRepos
    {
      Task<List<Product>> AddProductAsync(List<Product> productRequest);

        Task<Product?> UpdateProductPartialAsync(int id, ProductUpdateDto dto);

        Task<PaginatedResponseDto<Product>> GetProducts(string? category, int page = 1, int pageSize = 10);

        Task<List<Product>> SearchProducts(string search);
    }
}
