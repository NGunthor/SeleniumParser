using System.Data.Entity;

namespace AAAService
{
    public class Context : DbContext
    {
        public Context() : base(@"Data Source=.\SQLEXPRESS01;Initial Catalog=VkDB;Integrated Security=True")
        { }

        public DbSet<DbText> Texts { get; set; }
        public DbSet<DbLink> Links { get; set; }
        public DbSet<DbImage> Images { get; set; }
    }
}