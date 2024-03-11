using Shift_Tech.DbModels;

namespace Shift_Tech.Models.Contact
{
    public class RequestModel
    {
        public string RequestType { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string SenderName { get; set; }
    }
}
