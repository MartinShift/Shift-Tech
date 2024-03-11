using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shift_Tech.DbModels;
using Shift_Tech.Models.Cart;
using Shift_Tech.Models.Reviews;
using Shift_Tech.Models.ShopFilter;
using Shift_Tech.ViewModels;
using System.ComponentModel;
using System.Globalization;
using System.Security.Claims;

namespace Shift_Tech.Controllers
{
    public class ShopController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ShopDbContext _context;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ShopController(UserManager<User> userManager, SignInManager<User> signInManager, ShopDbContext context, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _roleManager = roleManager;
        }
        //Getters
        public IQueryable<Product> GetProducts() => _context.Products
                .Include(x => x.Category)
                .Include(x => x.Images)
            .Include(x => x.MainImage)
            .Include(x => x.Reviews)
            .Include(x => x.Purchases);
        public IQueryable<Category> GetCategories()
        {
            return _context.Categories.Include(x => x.Image).Include(x => x.Products);
        }
        public List<Category> GetTopCategories()
        => GetCategories()
            .OrderByDescending(x=> x.Products.Count)
                .Take(4)
                .ToList();
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
        public List<Product> GetCurrentPage(List<Product> products, int page)
        {
            return products.Skip((page - 1) * 9).Take(9).ToList();
        }
        public int GetPageCount(List<Product> products)
        {
            return products.Count / 9 + (products.Count % 9 == 0 ? 0 : 1);
        }
        public List<Product> GetTopProducts() =>
                GetProducts()
                .OrderByDescending(x => x.Purchases.Count)
                .Take(8)
                .ToList();
        public List<Product> GetRecentProducts() =>
            GetProducts()
            .OrderByDescending(x => x.Date)
            .Take(8)
            .ToList();
        public async Task<List<Category>> GetShopListCategories()
        {
            return await GetCategories()
                .OrderBy(x => x.Products.Count)
                .Take(10).ToListAsync();
        }
        public List<Product> FilterProductsByPrice(List<Product> products, PriceRange filter)
        {
            return products
              .Where(x =>
             x.Price >= filter.StartPrice && x.Price <= filter.EndPrice).ToList();
        }
        public List<Product> FilterProductsByCategory(List<Product> products, List<Category> selectedCategories)
        {
            if (selectedCategories.Count == 0) return products;
            return products
              .Where(x => selectedCategories.Any(c => c.Id == x.CategoryId)).ToList();
        }
        public PriceRange GetPriceRange(List<Product> products)
        {
            return new PriceRange() { StartPrice = (int)Math.Floor(products.Min(x => x.Price)), EndPrice = (int)Math.Floor(products.Max(x => x.Price)) };

        }

        //Home page
        public async Task<IActionResult> Index()
        {
            return View(new { FeaturedCategories = GetTopCategories(), FeaturedProducts = GetTopProducts(), RecentProducts = GetRecentProducts() });
        }
        [HttpGet]
        public IActionResult ProductDetail(int id)
        {
            var product = GetProducts().First(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return RedirectToAction("ShowProductDetail", new { productId = id });
        }
        [HttpGet]
        public IActionResult GetTopCategoriesView() =>
            View(
               GetCategories()
               .OrderByDescending(x => x.Products.Count)
               .Take(8)
               .ToList());
        [HttpGet("Shop/ProductDetail/{productId}")]
        public async Task<IActionResult> ShowProductDetail(int productId)
        {
            var product = GetProducts().First(x => x.Id == productId);
            var user = await _userManager.GetUserAsync(User);
            var reviews = _context.Reviews.Include(x => x.Product).Include(x => x.Publisher).ThenInclude(x => x.Logo).Where(x => x.Product.Id == productId).ToList();
            var review = reviews.FirstOrDefault(x => x.Publisher == user);
            ViewData["IsReviewed"] = review != null;
            ViewData["Reviews"] = reviews;
            if (product == null)
            {
                return NotFound();
            }
            var sameProducts = GetProducts().Where(x => x.Category == product.Category).Where(x => x.Id != productId).OrderByDescending(x => x.Purchases.Count).Take(6).ToList();
            return View("ProductDetail", new { Product = product, SameProducts = sameProducts });
        }

        //Review
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] AddReviewModel reviewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            var product = _context.Products.First(x => x.Id == reviewModel.productId);
            var review = new Review()
            {
                Description = reviewModel.Description,
                Rating = reviewModel.Rating,
                Date = DateTime.Now,
                Publisher = user,
                Product = product
            };
            _context.Reviews.Add(review);
            _context.SaveChanges();
            return Ok(new { Message = "Success!" });
        }

        //Cart
        [HttpGet]
        public async Task<IActionResult> GetTopListCategories()
        {
            var categories = GetCategories()
            .OrderByDescending(x => x.Products.Count)
        .Take(6);
        var newcategories = _context.Categories.Where(x => categories.Any(c => c.Id == x.Id)).ToList();
           return Ok(new
            {
                Categories = newcategories
           });
        }
        [HttpGet]
        public async Task<IActionResult> GetCartItemCount()
        {
            int itemcount = 0;
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var cart = GetCarts().FirstOrDefault(x => x.UserId == user.Id);

                itemcount = cart == null ? 0 : cart.Products.Count;
            }
            return Ok(new { Count = itemcount });
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = GetCarts().FirstOrDefault(x => x.UserId == user.Id);
            if (cart == null)
            {
                cart = new Cart()
                {
                    UserId = user.Id,
                };
                _context.Carts.Add(cart);
            }
            var product = await _context.Products.FindAsync(model.ProductId);
            var cartproduct = new CartProduct()
            {
                Product = product,
                ProductCount = model.ProductAmount
            };

            var find = cart.Products.FirstOrDefault(x => x.Product.Id == cartproduct.Product.Id);
            if (find != null)
            {
                find.ProductCount += cartproduct.ProductCount;
            }
            else
            {
                _context.CartProducts.Add(cartproduct);

                cart.Products.Add(cartproduct);
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}");
                }
            }
            _context.SaveChanges();
            int itemcount = 0;
            if (User.Identity.IsAuthenticated)
            {
                itemcount = cart == null ? 0 : cart.Products.Count;
            }
            return Ok(new { CartItemCount = itemcount });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart([FromBody] int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = GetCarts().FirstOrDefault(x => x.UserId == user.Id);
            var product = cart.Products.First(x => x.Id == productId);
            cart.Products.Remove(product);
            _context.SaveChanges();
            return Ok(new { Message = "Success!" });
        }
        [HttpPost]
        public async Task<IActionResult> CartItemCountChange([FromBody] AddToCartModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = GetCarts().FirstOrDefault(x => x.UserId == user.Id);
            var product = cart.Products.First(x => x.Id == model.ProductId);
            product.ProductCount = model.ProductAmount;
            _context.SaveChanges();
            return Ok(new { Message = "Success!" });
        }
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
           var user = await _userManager.GetUserAsync(User);
            var cart = GetCarts().FirstOrDefault(x => x.UserId == user.Id);
            if (cart == null)
            {
                cart = new Cart()
                {
                    UserId = user.Id,
                };
            }

            return View(cart);
        }

        //Product List
        [HttpGet]
        public async Task<IActionResult> ProductList()
        {
            var products = await GetProducts().Take(9).ToListAsync();
            var viewModel = new ProductListViewModel
            {
                Products = products,
                Categories = await GetShopListCategories(),
                SelectedCategories = new List<Category>(),
                PriceRange = GetPriceRange(products)
            };
            return View(viewModel);
        }
        [HttpGet("/Shop/CategoryProductList/{categoryId}")]
        public async Task<IActionResult> CategoryProductList(int categoryId)
        {
            var products = await GetProducts().Where(x=> x.Category.Id == categoryId).Take(9).ToListAsync();
            var viewModel = new ProductListViewModel
            {
                Products = products,
                Categories = await GetShopListCategories(),
                SelectedCategories = new List<Category>() { GetCategories().First(x => x.Id == categoryId) },
                PriceRange = GetPriceRange(products)
            };
            return View("ProductList",viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> FilterProducts([FromBody] SearchFilter filter)
        {
            var products = _context.Products
                .Include(x => x.Category)
                .Include(x => x.Images)
            .Include(x => x.MainImage)
            .Include(x => x.Reviews)
            .Include(x => x.Purchases)
            .Where(x => x.Price >= filter.MinPrice && x.Price <= filter.MaxPrice)
            .Where(x => x.Name.ToLower().Contains(filter.Search.ToLower()));
            if (filter.SelectedCategories.Count > 0)
            {
                products = products.Where(x => filter.SelectedCategories.Any(c => c == x.CategoryId));
            }
            switch (filter.SortOption)
            {
                case SortOption.Top:
                    products = products.OrderByDescending(x => x.Purchases.Count());
                    break;
                case SortOption.Recent:
                    products = products.OrderByDescending(x => x.Date);
                    break;
                case SortOption.CheapestToExpensive:
                    products = products.OrderBy(x => x.Price);
                    break;
                case SortOption.ExpensiveToCheapest:
                    products = products.OrderByDescending(x => x.Price);
                    break;
                case SortOption.ByRating:
                    products = products.OrderByDescending(x => x.Reviews.Average(r => r.Rating));
                    break;
            }
            ViewData["Products"] = products;

            products = products.Skip((filter.Page - 1) * 9)
            .Take(9);
            return PartialView("_ProductListPartial", await products.ToListAsync());
        }
    }
}

