using System.Data.Entity;

namespace EFTest
{
    public class TestContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
    }
}