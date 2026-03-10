using Microsoft.EntityFrameworkCore;

namespace MVCWEB.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Nothing's here
        }
       
    }
}
