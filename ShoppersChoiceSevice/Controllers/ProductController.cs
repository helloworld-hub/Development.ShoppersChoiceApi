using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppersChoice.DataAccess.NewFolder2;
using ShoppersChoice.Entities;
using ShoppersChoiceSevice.MySQLDbContext;

namespace ShoppersChoiceSevice.Controllers
{
    [ApiController]
    [Route("shopperschoiceservice/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly MySQLDBContext mySQLDBContext;

        private readonly IProductRepos productRepos;
        public ProductController(MySQLDBContext mySQLDBContext, IProductRepos productRepos)
        {
            this.mySQLDBContext = mySQLDBContext;
            this.productRepos = productRepos;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync()
        {
            var result = await mySQLDBContext.Products.ToListAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddProducts([FromBody] List<Product> product)
        {
            var result =await productRepos.AddProductAsync(product);

            return Ok(result);

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProductPartial(int id, [FromBody] ProductUpdateDto dto)
        { 
            var updatedProduct = await productRepos.UpdateProductPartialAsync(id, dto);

            if (updatedProduct == null)
                return NotFound();

            return Ok(updatedProduct);
        }


    }
}
