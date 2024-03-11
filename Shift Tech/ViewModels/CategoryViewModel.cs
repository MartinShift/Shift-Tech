using Shift_Tech.DbModels;

namespace Shift_Tech.ViewModels
{
    public class CategoryViewModel
    {
        public List<Category> Categories { get; set; }
        public int SelectedCategoryId { get; set; }
        public string NewCategoryName { get; set; }
    }

}
