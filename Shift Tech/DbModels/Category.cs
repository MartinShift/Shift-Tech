namespace Shift_Tech.DbModels
{
    public class Category 
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ImageFile? Image { get; set; }
        public virtual User Creator { get; set; }
    }
}
