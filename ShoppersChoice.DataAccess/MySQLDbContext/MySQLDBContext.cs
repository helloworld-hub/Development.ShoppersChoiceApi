using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShoppersChoice.Entities;

namespace ShoppersChoiceSevice.MySQLDbContext
{
    public class MySQLDBContext : DbContext
    {
        public MySQLDBContext(DbContextOptions<MySQLDBContext> options) : base(options)
        {
            
        }
        DbSet<Product> Products { get; set; }


    }
}
