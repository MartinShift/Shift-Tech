using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shift_Tech.DbModels;
using Shift_Tech.Migrations;
using Shift_Tech.Models.Contact;
using System.Net.Mail;
using System.Net;

namespace Shift_Tech.Controllers
{
    public class ContactController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ShopDbContext _context;
        public ContactController(UserManager<User> userManager, SignInManager<User> signInManager, ShopDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SubmitContact([FromForm] ContactModel model)
        {
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
<p><span class=""""email-address"""">{model.Email}</span></p>
    <p>{model.Message}</p>   
    <p><em>— User: {model.SenderName}</em></p>
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
                    From = new MailAddress($"{model.Email}", $"{model.SenderName}"),
                    Subject = model.Subject,
                    Body = htmlContent,
                };
                mail.To.Add($"smtptestemail128@gmail.com");
                smtpclient.Send(mail);
            }
            catch (Exception)
            {

            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Request()
        {
            return View();
        }

        public async Task<IActionResult> SubmitRequest([FromForm] Request request)
        {
            var user =await _userManager.GetUserAsync(User);
            request.Sender = user;
            _context.Requests.Add(request);
            _context.SaveChanges();
            return Redirect("/");
        }
    }
}
