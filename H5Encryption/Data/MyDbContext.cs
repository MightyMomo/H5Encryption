using Microsoft.EntityFrameworkCore;
using H5Encryption.Models.DB;

namespace H5Encryption.Data
{
    public class MyDbContext:DbContext
    {
        public DbSet<User> User  { get; set; }
        public DbSet<TodoItem> TodoItem { get; set; }

       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source = .; Initial Catalog = H5Encryption; Integrated Security=true;");
        }

        public MyDbContext(DbContextOptions<MyDbContext> options):base(options) { }
    }
}
