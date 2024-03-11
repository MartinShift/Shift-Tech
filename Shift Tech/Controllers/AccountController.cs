using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shift_Tech.DbModels;
using Shift_Tech.Models.Account;
using System.Security.Claims;

namespace Shift_Tech.Controllers
{
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ShopDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ShopDbContext shopDbContext, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = shopDbContext;
            _webHostEnvironment = webHostEnvironment;
            _roleManager = roleManager;
        }
        public IQueryable<Product> GetProducts() => _context.Products
        .Include(x => x.Category)
        .Include(x => x.Images)
    .Include(x => x.MainImage)
    .Include(x => x.Reviews)
    .Include(x => x.Purchases);
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        public string GetFirstPartOfEmail(string email)
        {

            if (email != null)
            {
                string[] emailParts = email.Split('@');

                if (emailParts.Length >= 1)
                {
                    // The first part of the email is in emailParts[0]
                    string firstPartOfEmail = emailParts[0];

                    return firstPartOfEmail;
                }
            }
            return "";
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            foreach (var key in ModelState.Keys)
            {
                var entry = ModelState[key];
                if (entry.Errors.Any())
                {
                    foreach (var error in entry.Errors)
                    {
                        // You can log or print the validation error messages here
                        Console.WriteLine($"Property: {key}, Error: {error.ErrorMessage} ");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var existingLogin = await _userManager.FindByNameAsync(model.Login);
                if (existingLogin != null)
                {
                    ModelState.AddModelError(string.Empty, "The login is already in use.");
                }

                var existingEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError(string.Empty, "The email is already in use.");
                }

                if (ModelState.ErrorCount > 0)
                {
                    // Return the error messages as JSON response
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new { Errors = errors });
                }

                var user = new User { VisibleName = model.Login, UserName = model.Login, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Ok(new { Message = "Success!", Errors = new List<string> { } });

                }

                // Handle other registration errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return BadRequest(new { Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();

        }
        [HttpPost]
        [AllowAnonymous]
         public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var existingEmail = await _userManager.FindByEmailAsync(model.LoginOrEmail);
                var existingLogin = await _userManager.FindByNameAsync(model.LoginOrEmail);
                if (existingLogin == null && existingEmail == null)
                {
                    return BadRequest(new { Message = "", Error = "No Such Login Exists" });
                }
                if (existingEmail != null) { model.LoginOrEmail = existingEmail.UserName; }

                var result = await _signInManager.PasswordSignInAsync(model.LoginOrEmail, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var editPageUrl = "/";
                 
                 
                    if (!string.IsNullOrEmpty(editPageUrl))
                    {
                        return Ok(new { Message = "Success!", Error = "" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return BadRequest(new { Message = "", Error = "Wrong Password!" });
                }
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Message = "Success!" });
        }

        [Authorize]
        public IActionResult SecurePage()
        {
            return View();
        }
    
        public async Task<IActionResult> UserProfile(int id)
        {
            var user = _context.Users.Include(x => x.Logo).Include(x=> x.CreatedProducts).First(x => x.Id == id);
            var products = GetProducts().Where(x=> x.Creator.Id == id).ToList();
            double averageRating = 0;
            if (products.Any(x => x.Reviews.Count > 0))
            {
                var averageproducts = products.Where(x => x.Reviews.Count > 0).Select(x => x.Reviews.Average(r => r.Rating));
                    averageRating = Math.Floor(averageproducts.Average()*10) / 10;
            }       
            var userProfile = new UserProfile
            {
                Name = user.VisibleName,
                Login = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                LogoUrl = user.Logo == null ? "https://upload.wikimedia.org/wikipedia/commons/thumb/6/65/No-Image-Placeholder.svg/1200px-No-Image-Placeholder.svg.png " : user.Logo.Url(),
                IsAdmin = User.IsInRole("Admin"),
                IsSeller = User.IsInRole("Seller"),
                AverageRating = averageRating,
                Products = products.OrderByDescending(x=> x.Purchases.Count).Take(8).ToList()
            };

            return View(userProfile);
        }
        public async Task<IActionResult> ViewProfile(int userId)
        {
            return RedirectToAction("UserProfile", new { id = userId });
        } 
        [Authorize]
        public async Task<IActionResult> MyProfile()
        {
            var usr = await _userManager.GetUserAsync(User);
            return RedirectToAction("UserProfile",new { id = usr.Id });
        }

        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var usr = await _userManager.GetUserAsync(User);
            var user = _context.Users.Include(x => x.Logo).First(x => x.UserName == usr.UserName);
            var userProfile = new UserProfile
            {
                Name = user.VisibleName,
                Login = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                LogoUrl = user.Logo == null ? "https://upload.wikimedia.org/wikipedia/commons/thumb/6/65/No-Image-Placeholder.svg/1200px-No-Image-Placeholder.svg.png " : user.Logo.Url()
            };

            return View(userProfile);
        }
        [HttpPost("/Account/Uploads/")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var usr = await _userManager.GetUserAsync(User);
            var user = _context.Users.Include(x => x.Logo).First(x => x.UserName == usr.UserName);
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var dbFile = new ImageFile()
            {
                Filename = filename,
                RootDirectory = "Uploads",
            };
            var localFilename =
                Path.Combine(_webHostEnvironment.WebRootPath, dbFile.RootDirectory, dbFile.Filename);
            using (var localFile = System.IO.File.Open(localFilename, FileMode.OpenOrCreate))
            {
                await file.CopyToAsync(localFile);
            }
            _context.Images.Add(dbFile);
            user.Logo = dbFile;
            await _context.SaveChangesAsync();
            return Ok(new { url = dbFile.Url() });
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile([FromBody] UserProfile model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    // Update user properties
                    user.VisibleName = model.Name;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;

                    // Update the user in the database
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Ok(new { Message = "Success!" });
                    }
                    else
                    {
                        // Handle errors and return appropriate response
                        return BadRequest(new { Message = "Failed to update profile", Errors = result.Errors });
                    }
                }
                else
                {
                    return NotFound(new { Message = "User not found" });
                }
            }

            return BadRequest(new { Message = "Invalid model state" });
        }

        [HttpPost("GoogleLogin")]
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleLoginCallback", "Account", null, protocol: HttpContext.Request.Scheme);
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        [HttpGet("GoogleLoginCallback")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }
            var user = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
            var login = GetFirstPartOfEmail(info.Principal.FindFirstValue(ClaimTypes.Email));
            if (user == null)
            {
                user = new User
                {
                    UserName = login,
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    VisibleName = info.Principal.FindFirstValue(ClaimTypes.Name)
                };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
            }
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Shop");
        }
    }
}
