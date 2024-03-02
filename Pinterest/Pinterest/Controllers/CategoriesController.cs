using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pinterest.Data;
using Pinterest.Models;

namespace Pinterest.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;
        private IWebHostEnvironment _env;

        private readonly UserManager<AppUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CategoriesController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(User).Result;

            var categories = db.Categories
                .Where(c => c.AppUserId == user.Id)
                .ToList();

            ViewBag.Categories = categories;
            ViewBag.User = user;

            return View();
        }

        [HttpGet]
        public IActionResult Show(int? id)
        {
            var pins = from pc in db.PinCategories
                       where pc.CategoryId == id
                       join p in db.Pins on pc.PinId equals p.Id
                       select p;

            var categories = from c in db.Categories
                             where c.Id == id
                             select c;

            ViewBag.Pins = pins.ToList();
            ViewBag.Category = categories.SingleOrDefault();

            return View();
        }

        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> New(Category category)
        {
            var user = await _userManager.GetUserAsync(User);

            category.AppUserId = user.Id;

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            db.Categories.Add(category);

            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SavePin([FromForm] int categoryId, [FromForm] int pinId)
        {
            PinCategory newPinCategory = new PinCategory();
            newPinCategory.CategoryDate = DateTime.Now;
            newPinCategory.CategoryId = categoryId;
            newPinCategory.PinId = pinId;

            db.PinCategories.Add(newPinCategory);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int categoryId, [FromForm] int pinId)
        {
            PinCategory pincategory = db.PinCategories.FirstOrDefault(pc => pc.CategoryId == categoryId && pc.PinId == pinId);

            Category category = db.Categories.Find(pincategory.CategoryId);

            AppUser currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != category.AppUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            db.PinCategories.Remove(pincategory);
            db.SaveChanges();
            return RedirectToAction("Show", new { id = category.Id });
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete_Category(int categoryId)
        {
            Category category = db.Categories.Find(categoryId);

            AppUser currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != category.AppUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
