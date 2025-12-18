using ShoppersChoice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppersChoice.DataAccess.NewFolder2
{
    public interface IProductRepos
    {
      Task<List<Product>> AddProductAsync(List<Product> productRequest);

        Task<Product?> UpdateProductPartialAsync(int id, ProductUpdateDto dto);
    }
}
