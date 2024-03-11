using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shift_Tech.DbModels
{
    public class Cart
    {
        public Cart()
        {
            Products = new HashSet<CartProduct>();
        }
        [Key]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public virtual ICollection<CartProduct> Products { get; set; }
    }
}
