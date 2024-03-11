using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shift_Tech.DbModels;
using System.Net.Mail;
using System.Net;
using Shift_Tech.Models.Liqpay;

namespace Shift_Tech.Controllers
{
    public class CheckoutController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly ShopDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly Models.Liqpay.LiqPay _liqPay;
		public CheckoutController(UserManager<User> userManager, SignInManager<User> signInManager, ShopDbContext context, IWebHostEnvironment webHostEnvironment, Models.Liqpay.LiqPay liqPay)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_context = context;
			_webHostEnvironment = webHostEnvironment;
			_liqPay = liqPay;
		}
		public List<Cart> GetCarts()
		{
			return _context.Carts
				 .Include(x => x.User)
				 .Include(x => x.Products)
				 .ThenInclude(x => x.Product)
				 .ThenInclude(product => product.MainImage)
				 .Include(x => x.Products)
				 .ThenInclude(x => x.Product)
				 .ThenInclude(product => product.Category)
					.Include(x => x.Products)
				 .ThenInclude(x => x.Product)
				 .ThenInclude(product => product.Reviews)
					 .Include(x => x.Products)
				 .ThenInclude(x => x.Product)
				 .ThenInclude(product => product.Images)
				 .ToList();
		}
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);
			var carts = GetCarts();
			var cart = carts.First(x => x.User.Id == user.Id);

			return View(cart);
		}

		[HttpPost]
		public async Task<IActionResult> SubmitCheckout([FromForm] Order order)
		{
			var user = await _userManager.GetUserAsync(User);
			var cart = GetCarts().First(x => x.User.Id == user.Id);

			var products = cart.Products.Select(x => new OrderProduct() { Product = x.Product, ProductCount = x.ProductCount }).ToList();
			order.Guid = Guid.NewGuid().ToString();
			order.User = user;
			order.Email = user.Email;
			order.CreationDate = DateTime.Now;
			order.Status = Status.NotPaid;
			order.Products = products;
			_context.Orders.Add(order);
			_context.SaveChanges();
			return RedirectToAction("Payment", new { orderId = order.Id });
		}
		public async Task<IActionResult> Payment(int orderId)
		{
			var order = _context.Orders.Include(x => x.Products).ThenInclude(x => x.Product).FirstOrDefault(x => x.Id == orderId);
			var param = _liqPay.PayParams(Convert.ToDecimal(order.TotalPriceWithShipping()), "Order products", order.Guid);
			ViewData["order"] = orderId;
			ViewData["data"] = _liqPay.GetData(param);
			ViewData["signature"] = _liqPay.GetSignature(param);
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> SubmitPayment([FromBody] Notify notify)
		{
			var order = _context.Orders.Include(x => x.Products).ThenInclude(x => x.Product).FirstOrDefault(x => x.Id == notify.OrderId);
			var user = await _userManager.GetUserAsync(User);
			var cart = GetCarts().First(x => x.User.Id == user.Id);
			cart.Products.Clear();
			order.Products.ToList().ForEach(x => x.Product.InStock -= x.ProductCount);
			order.Status = Status.Paid;
            var message = "Thank you for purchasing in Shift Tech! Your order is being reviewed and will be sent soon";
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
                    Credentials = new NetworkCredential("mori.steamer@gmail.com", System.IO.File.ReadAllText("D:\\SecureFiles\\SmtpPassword.txt")),
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
            _context.SaveChanges();
			return Ok();
		}
		[HttpPost]
		public async Task<IActionResult> CancelPayment(int orderId)
		{
			var order = _context.Orders.Include(x => x.Products).ThenInclude(x => x.Product).FirstOrDefault(x => x.Id == orderId);
			_context.Orders.Remove(order);
			_context.SaveChanges();
			return Ok();
		}


	}
}
