using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shift_Tech.DbModels;
using Shift_Tech.Models.Cart;
using Shift_Tech.Models.Reviews;
using Shift_Tech.Models.ShopFilter;
using Shift_Tech.ViewModels;
using System.ComponentModel;
using System.Security.Claims;

namespace Shift_Tech.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RequestController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public RequestController(ShopDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> DeclineRequest([FromBody]  int requestId)
        {
            var request = _context.Requests.FirstOrDefault(x => x.Id == requestId);
            if (request != null)
            {
                _context.Requests.Remove(request);
                _context.SaveChanges();
            }
            return Redirect("/Request");
        }


        public async Task<IActionResult> AcceptRequest([FromBody]int requestId)
        {
            var request = _context.Requests.Include(x => x.Sender).FirstOrDefault(x => x.Id == requestId);
            if (request != null)
            {

                switch (request.RequestType)
                {
                    case RequestType.SellerRequest:
                        bool sellerRoleExists = await _roleManager.RoleExistsAsync("Seller");
                        if(!sellerRoleExists)
                        {
                            var role = new IdentityRole<int>
                            {
                                Name = "Seller"
                            };
                            await _roleManager.CreateAsync(role);
                        }
                        await _userManager.AddToRoleAsync(request.Sender, "Seller");
                        await _userManager.AddClaimAsync(request.Sender, new Claim(ClaimTypes.Role, "Seller"));
                        break;
                    case RequestType.AdminRequest:
                        bool adminRoleExists = await _roleManager.RoleExistsAsync("Admin");
                        if (!adminRoleExists)
                        {
                            var role = new IdentityRole<int>
                            {
                                Name = "Admin"
                            };
                            await _roleManager.CreateAsync(role);
                        }
                        await _userManager.AddToRoleAsync(request.Sender, "Admin");
                        await _userManager.AddClaimAsync(request.Sender, new Claim(ClaimTypes.Role, "Admin"));
                        break;
                }
                _context.Requests.Remove(request);
                _context.SaveChanges();
            }
            return Redirect("/Request");
        }
        public IActionResult Index()
        {
            return View(_context.Requests.Include(x=> x.Sender).ThenInclude(x=> x.Logo).ToList());
        }


    }
}
