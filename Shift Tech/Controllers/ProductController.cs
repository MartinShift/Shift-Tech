using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Shift_Tech.ViewModels;
using Shift_Tech.DbModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shift_Tech.Models.Categories;
using Microsoft.AspNetCore.Authorization;

namespace Shift_Tech.Controllers
{
    [Authorize(Roles = "Seller")]
    public class ProductController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ShopDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }
        public List<Product> GetProducts() => _context.Products
        .Include(x => x.Category)
        .Include(x => x.Images)
    .Include(x => x.MainImage)
    .Include(x => x.Reviews)
    .Include(x => x.Purchases)
            .Include(x => x.Creator)
    .ToList();
        public async Task<IActionResult> EditProducts()
        {
            var user = await _userManager.GetUserAsync(User);
            var products = GetProducts().Where(x => x.Creator == user).ToList();
            var categories = _context.Categories.Include(x => x.Image).Include(x => x.Products).ToList();
            return View(new { Products = products, Categories = categories });
        }
        public IActionResult EditProductDetail(int productId)
        {
            var product = GetProducts().First(x => x.Id == productId);

            return View(new { Product = product, Categories = _context.Categories });
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] AddModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.Name))
                {
                    var newProduct = new Product
                    {
                        Name = model.Name,
                        Price = model.Price,
                        PreviousPrice = model.Price,
                        Creator = user,
                        Date = DateTime.Now,
                        InStock = 0,
                        Description = "",
                        ShortDescription = "",
                        Category = _context.Categories.First(x => x.Id == model.CategoryId)
                    };
                    _context.Products.Add(newProduct);
                    _context.SaveChanges();
                }
            }
            return Ok(new { Message = "Success!" });
        }
        [HttpPost]
        public IActionResult DeleteProduct([FromBody] int productId)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == productId);
            if (product != null)
            {
                product.Reviews.Clear();
                product.Purchases.Clear();
                _context.Products.Remove(product);
            }
            _context.SaveChanges();
            return Ok(new { Message = "Success!" });
        }
        [HttpPost]
        public IActionResult UpdateProduct([FromBody] Product updatedProduct)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == updatedProduct.Id);
            if (product != null)
            {
                product.PreviousPrice = updatedProduct.PreviousPrice;
                product.Price = updatedProduct.Price;
                product.Name = updatedProduct.Name;
                product.Description = updatedProduct.Description;
                product.ShortDescription = updatedProduct.ShortDescription;
                product.CategoryId = updatedProduct.CategoryId;
                product.InStock = updatedProduct.InStock;
            }
            _context.SaveChanges();
            return Ok("Success!"); // Replace with your desired action
        }

        public void ClearMainImage(int productId)
        {
            var product = _context.Products.Include(x => x.MainImage).First(x => x.Id == productId);

            var path = product.MainImage?.Path();
            if (path != null)
            {
                product.MainImage = null;
                System.IO.File.Delete(path);
            }
        }
        public void ClearImages(int productId)
        {
            var product = _context.Products.Include(x => x.Images).First(x => x.Id == productId);
            if (product.Images.Count != 0)
            {
                var paths = product.Images.Select(x => x.Path()).ToList();

                product.Images.Clear();

                paths.ForEach(path => System.IO.File.Delete(path));
            }
        }

        [HttpPost("/Product/Uploads/{id}")]
        public async Task<IActionResult> Upload(IFormFile file, int id)
        {
            var current = _context.Products.First(x => x.Id == id);
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
            ClearMainImage(id);
            current.MainImage = dbFile;
            await _context.SaveChangesAsync();
            return Ok(dbFile);
        }
        [HttpPost("/Product/UploadsMultiple/{id}")]
        public async Task<IActionResult> UploadMultiple(ICollection<IFormFile> files, int id)
        {
            var current = _context.Products.First(x => x.Id == id);
            var uploadedFiles = new List<ImageFile>();

            foreach (var file in files)
            {
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var dbFile = new ImageFile()
                {
                    Filename = filename,
                    RootDirectory = "Uploads",
                };
                var localFilename = Path.Combine(_webHostEnvironment.WebRootPath, dbFile.RootDirectory, dbFile.Filename);
                using (var localFile = System.IO.File.Open(localFilename, FileMode.OpenOrCreate))
                {
                    await file.CopyToAsync(localFile);
                }
                _context.Images.Add(dbFile);
                uploadedFiles.Add(dbFile);
            }
            ClearImages(id);
            uploadedFiles.ForEach(x =>
            {
                current.Images.Add(x);
            });

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Success!" });
        }
    }
}
