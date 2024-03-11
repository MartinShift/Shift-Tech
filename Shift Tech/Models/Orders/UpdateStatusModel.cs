using Shift_Tech.DbModels;

namespace Shift_Tech.Models.Orders
{
    public class UpdateStatusModel
    {
        public Status Status { get; set; }
        public int OrderId { get; set; }
    }
}
