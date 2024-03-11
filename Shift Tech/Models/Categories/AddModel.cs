using Microsoft.AspNetCore.Mvc;

namespace Shift_Tech.Models.Categories
{
    public class AddModel
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
    }
}
