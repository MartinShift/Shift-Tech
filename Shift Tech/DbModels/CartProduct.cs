namespace Shift_Tech.DbModels
{
    public class CartProduct
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int ProductCount { get; set; }
    }
}
