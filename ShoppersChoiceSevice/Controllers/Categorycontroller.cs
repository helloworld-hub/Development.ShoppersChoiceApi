using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ShoppersChoiceSevice.MySQLDbContext;

namespace ShoppersChoiceSevice.Controllers
{
    [Route("shopperschoiceservice/[controller]")]
    public class Categorycontroller : ControllerBase
    {
        private readonly MySQLDBContext mySQLDbContext;

        public Categorycontroller(MySQLDBContext mySQLDbContext)
        {
            this.mySQLDbContext = mySQLDbContext;   
        }
       
            [HttpGet]
            public async Task<IActionResult> GetProductsAsync()
            {
                var result = await mySQLDbContext.Categories.ToListAsync();
                return Ok(result);
            }
    }
}
