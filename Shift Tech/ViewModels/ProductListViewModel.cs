using Shift_Tech.DbModels;
using Shift_Tech.Models.ShopFilter;

namespace Shift_Tech.ViewModels
{
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<Category> SelectedCategories { get; set; }
        public PriceRange PriceRange { get; set; }
    }
}
