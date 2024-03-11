namespace Shift_Tech.DbModels
{
    public enum RequestType
    {
        SellerRequest,AdminRequest,
    }
    public class Request
    {
        public int Id { get; set; }
        public User Sender { get; set; }
        public RequestType RequestType { get; set;}
        public string Message { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string SenderName { get; set; }
    }
}
