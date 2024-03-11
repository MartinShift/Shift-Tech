 using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Shift_Tech.DbModels
{
    public class ShopDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {
        }
        public ShopDbContext() : base() { }
        public virtual DbSet<ImageFile> Images { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CartProduct> CartProducts { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
		public virtual DbSet<OrderProduct> OrderProducts { get; set; }
        public virtual DbSet<Request> Requests { get; set; }
	}
}
