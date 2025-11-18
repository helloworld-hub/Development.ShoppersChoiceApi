using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppersChoice.Entities;
using ShoppersChoiceSevice.MySQLDbContext;

namespace ShoppersChoiceSevice.Controllers
{
    [ApiController]
    [Route("shopperschoiceservice/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly MySQLDBContext mySQLDBContext;
        public ProductController(MySQLDBContext mySQLDBContext)
        {
            this.mySQLDBContext = mySQLDBContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync()
        {
            var result = await mySQLDBContext.Products.ToListAsync();
            return Ok(result);
        }

    }
}
