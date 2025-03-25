using AMoverGRPC.Models;
using Microsoft.EntityFrameworkCore;

namespace AMoverGRPC.Data
{
    public class MotaDbContext : DbContext
    {
        public MotaDbContext(DbContextOptions<MotaDbContext> options) : base(options) { }

        public DbSet<Mota> Motas { get; set; }

    }
}