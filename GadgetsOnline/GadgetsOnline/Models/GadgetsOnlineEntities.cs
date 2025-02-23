using System.Data.Entity;
using GadgetsOnline;
namespace GadgetsOnline.Models
{
    public class GadgetsOnlineEntities : DbContext
    {
        public GadgetsOnlineEntities() : base(ConnStringHelper.GetConnectionString())
        {
            Database.SetInitializer(new GadgetsOnlineDBInitializer());
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}