using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pinterest.Data;
using Pinterest.Models;
using Pinterest.Services;
using Pinterest.Repositories;

namespace Pinterest.Controllers;

public class CategoriesController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly CategoryService _categoryService;

    public CategoriesController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
            
        _env = env;
        _userManager = userManager;
        _roleManager = roleManager;

        var categoryRepo = new CategoryRepository(context);
        _categoryService = new CategoryService(categoryRepo);
    }

    [Authorize(Roles = "User")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var categories = _categoryService.GetCategoriesForUser(user.Id);

        ViewBag.Categories = categories;
        ViewBag.User = user;

        return View();
    }

    [HttpGet]
    public IActionResult Show(int? id)
    {
        var (pins, category) = _categoryService.GetCategoryWithPins(id);

        ViewBag.Pins = pins;
        ViewBag.Category = category;

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

        _categoryService.AddCategory(category);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult SavePin([FromForm] int categoryId, [FromForm] int pinId)
    {
        _categoryService.SavePinToCategory(categoryId, pinId);
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPost]
    public async Task<IActionResult> Delete([FromForm] int categoryId, [FromForm] int pinId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var success = _categoryService.RemovePinFromCategory(categoryId, pinId, currentUser.Id, User.IsInRole("Admin"));

        if (!success)
        {
            return Forbid();
        }

        return RedirectToAction("Show", new { id = categoryId });
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPost]
    public async Task<IActionResult> Delete_Category(int categoryId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var success = _categoryService.DeleteCategory(categoryId, currentUser.Id, User.IsInRole("Admin"));

        if (!success)
        {
            return Forbid();
        }

        return RedirectToAction("Index");
    }
}