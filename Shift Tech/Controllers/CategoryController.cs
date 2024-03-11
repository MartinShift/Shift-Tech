using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shift_Tech.DbModels;
using Shift_Tech.ViewModels;

namespace Shift_Tech.Controllers
{
    [Authorize(Roles ="Seller")]
    public class CategoryController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CategoryController(ShopDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }

        private async Task<User> GetUser()
        {
            return await _userManager.GetUserAsync(User);

        }
        public async Task<IActionResult> EditCategories()
        {
       
            var user = await GetUser();
            var categories = _context.Categories.Include(x => x.Image).Include(x => x.Products).Include(x=> x.Creator).Where(x => x.Creator == user).ToList();
            var viewModel = new CategoryViewModel
            {
                Categories = categories
            };
            return View(viewModel);
        }
        [HttpPost("/Category/Uploads/{id}")]
        public async Task<IActionResult> Upload(IFormFile file, int id)
        {
            var current = _context.Categories.First(x => x.Id == id);
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
            current.Image = dbFile;
            await _context.SaveChangesAsync();
            return Ok(dbFile);
        }
        [HttpPost]
        public async Task<IActionResult> AddCategoryAsync([FromBody] string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (_context.Categories.Any(x => x.Name.ToLower() == name.ToLower()))
                {
                    return BadRequest(new { Message = "This Category Already Exists!" });
                }
                var user = await _userManager.GetUserAsync(User);
                var category = new Category { Name = name, Creator = user };
                _context.Categories.Add(category);
                _context.SaveChanges();
            }
            return Ok(new { Message = "Success!" });
        }

        [HttpPost]
        public IActionResult EditCategoryName([FromBody] Category category)
        {
            if(_context.Categories.Any(x=> x.Name.ToLower()==category.Name))
            {
                return View();
            }    
            var existingCategory = _context.Categories.Find(category.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
            }

            _context.SaveChanges();

            return RedirectToAction("EditCategory");
        }
        [HttpPost]

        public IActionResult DeleteCategory([FromBody] int id)
        {
            var category = _context.Categories.First(x => x.Id == id);
            if (category == null)
            {
                return BadRequest("Category Not Found!");
            }

            if (_context.Products.Any(p => p.Category.Id == id))
            {
                var viewModel = new CategoryViewModel
                {
                    Categories = _context.Categories.ToList()
                };
                return BadRequest("Cannot delete a category with associated products.");
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok(new { Message = "Success!" });
        }
    }

}
