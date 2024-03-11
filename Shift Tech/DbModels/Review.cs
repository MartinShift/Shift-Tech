namespace Shift_Tech.DbModels
{
    public class Review
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }
        public DateTime Date { get; set; }
        public User Publisher { get; set; }
        public Product Product { get; set; }
    }
}
