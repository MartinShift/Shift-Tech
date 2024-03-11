namespace Shift_Tech.Models.ShopFilter
{
    public enum SortOption
    {
        Top, Recent, CheapestToExpensive, ExpensiveToCheapest, ByRating
    };
    public class SearchFilter
    {
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public List<int> SelectedCategories { get; set; }
        public string Search { get; set; }
        public int Page { get; set; }
        public SortOption SortOption { get; set; }
    }
}
