using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace Shift_Tech.DbModels
{
    public class Product
    {
        public Product()
        {
            Purchases = new HashSet<CartProduct>();
            Reviews = new HashSet<Review>();
            Images = new HashSet<ImageFile>();
        }
        public int Id { get; set; }
        public double Price { get; set; }
        public double PreviousPrice { get; set; }

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string? Description { get; set; }

        public int InStock { get; set; }
        public DateTime Date { get; set; }

        public User Creator { get; set; }
        public ImageFile? MainImage { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
        public virtual ICollection<CartProduct> Purchases { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<ImageFile> Images { get; set; }
      
    }
}
