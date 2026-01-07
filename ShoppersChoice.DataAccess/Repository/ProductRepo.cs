
using Microsoft.EntityFrameworkCore;
using ShoppersChoice.DataAccess.NewFolder2;
using ShoppersChoice.Entities;
using ShoppersChoice.Entities.DTOs;
using ShoppersChoiceSevice.MySQLDbContext;

namespace ShoppersChoice.DataAccess.NewFolder1
{
    public class ProductRepo : IProductRepos
    {
        private readonly MySQLDBContext mySQLDBContext;
        public ProductRepo(MySQLDBContext mySQLDBContext) 
        {
            this.mySQLDBContext = mySQLDBContext;
        }

       public async Task<List<Product>> AddProductAsync(List<Product> productRequest)
        {  

            await mySQLDBContext.AddRangeAsync(productRequest);
            await mySQLDBContext.SaveChangesAsync();

            return productRequest;
        }

        public async Task<Product?> UpdateProductPartialAsync(int id, ProductUpdateDto dto)
        {
            var product = await mySQLDBContext.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return null;

            if (dto.name != null) product.name = dto.name;
            if (dto.price.HasValue) product.price = dto.price.Value;
            if (dto.discountPercent.HasValue) product.discountPercent = dto.discountPercent.Value;
            if (dto.category != null) product.category = dto.category;
            if (dto.images != null) product.images = dto.images;
            if (dto.stock.HasValue) product.stock = dto.stock.Value;

            await mySQLDBContext.SaveChangesAsync();
            return product;
        }

        public async Task<PaginatedResponseDto<Product>> GetProducts(string? category, int page = 1, int pageSize = 10)
        {
            var query = mySQLDBContext.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.category == category);

            int totalRecords = query.Count();

            var products = query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PaginatedResponseDto<Product>
            {
                    Data = products,       
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize
            };

            return response;

        }

        public async Task<List<Product>> SearchProducts(string search)
        {
            return await mySQLDBContext.Products
                .Where(p => p.name.Contains(search))
                .OrderBy(p => p.name)
                .Take(10)
                .ToListAsync();
        }


    }
}
