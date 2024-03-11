using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shift_Tech.DbModels
{
    public class User : IdentityUser<int>
    {
        public User() 
        {
            CreatedProducts = new HashSet<Product>();
            CreatedCategories = new HashSet<Category>();
        }
        public virtual ImageFile? Logo { get; set; }
        public string VisibleName { get; set; }
        public virtual Cart Cart { get; set; }
        [InverseProperty(nameof(Product.Creator))]
        public virtual ICollection<Product> CreatedProducts { get; set; }
        public virtual ICollection<Category> CreatedCategories { get; set; }
    }
}
