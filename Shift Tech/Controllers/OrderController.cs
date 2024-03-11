using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shift_Tech.DbModels;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using Shift_Tech.Models.Orders;
using Microsoft.AspNetCore.Authorization;

namespace Shift_Tech.Controllers
{
    [Authorize(Roles ="Admin")]
    public class OrderController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ShopDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public OrderController(UserManager<User> userManager, SignInManager<User> signInManager, ShopDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> ManageOrders()
        {
            return View(_context.Orders.Include(x => x.Products).ThenInclude(x => x.Product).ToList());
        }
        public async Task<IActionResult> EditOrderDetail(int orderId)
        {
            return View(_context.Orders.Include(x => x.Products).ThenInclude(x => x.Product).First(x => x.Id == orderId));
        }
        public IActionResult DeleteOrder([FromBody] int orderId)
        {
            var order = _context.Orders.Include(x => x.Products).First(x => x.Id == orderId);
            order.User = null;
            _context.Orders.Remove(order);
            _context.SaveChanges();
            return Ok();
        }

        public async Task<IActionResult> UpdateOrder([FromBody] UpdateStatusModel updatedOrder)
        {
            var order = _context.Orders.Include(x => x.Products).ThenInclude(x => x.Product).FirstOrDefault(x => x.Id == updatedOrder.OrderId);
            var message = "";

            switch (updatedOrder.Status)
            {
                case Status.NotPaid:
                    message = $"Created order with Id {order.Guid}";
                    break;
                case Status.Paid:
                    message = "Thank you for purchasing in Shift Tech! Your order is being reviewed and will be sent soon";
                    break;
                case Status.Sent:
                    message = "Your product is being sent. It can take a few days for the product to deliver.";
                    break;
                case Status.Received:
                    message = $"Your product is delivered at address {order.AddressLine1}, {order.City}";
                    break;
            }
            string htmlContent = $@"
     <!DOCTYPE html>
<html>
<head>
    <style>
        body {{{{
            text-align: center;
        }}}}
        
        .email-address {{{{
            color: #777777; 
            font-size: 18px; 
        }}}}
    </style>
</head>
<body>
    <p>{message}</p>
    <p><span class=""""email-address"""">{order.Email}</span></p>
    <p>Best regards,</p>
    <p><em>— Team Shift Tech</em></p>
</body>
</html>
";
            try
            {
                var smtpclient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("mori.steamer@gmail.com", System.IO.File.ReadAllText("D:\\SecureFiles\\SmtpPassword")),
                    EnableSsl = true,

                };
                var mail = new MailMessage()
                {
                    IsBodyHtml = true,
                    From = new MailAddress("mori.steamer@gmail.com", "Shift Support"),
                    Subject = "Shift Tech",
                    Body = htmlContent,
                };
                mail.To.Add($"{order.Email}");
                smtpclient.Send(mail);
            }
            catch (Exception)
            {

            }
            return View();
        }
    }
}
