using Pinterest.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Pinterest.Data;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Pinterest.Services;
using Pinterest.Repositories;

namespace Pinterest.Controllers
{
    public class PinsController : Controller
    {
        private readonly ApplicationDbContext db;
        private IWebHostEnvironment _env;

        private readonly UserManager<AppUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PinService _pinService;

        public PinsController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;

            var repo = new PinRepository(context);
            _pinService = new PinService(repo);

        }

        [HttpGet]
        public IActionResult Index()
        {
            int _perPage = 3;
            int currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            string search = Convert.ToString(HttpContext.Request.Query["search"])?.Trim() ?? "";

            var (pins, lastPage, paginationUrl) = _pinService.GetPins(search, currentPage == 0 ? 1 : currentPage, _perPage);

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }

            ViewBag.SearchString = search;
            ViewBag.Pins = pins;
            ViewBag.lastPage = lastPage;
            ViewBag.PaginationBaseUrl = paginationUrl;

            return View();
        }

        [HttpGet]
        public IActionResult IndexRecent()
        {
            int _perPage = 3;
            int currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            string search = Convert.ToString(HttpContext.Request.Query["search"])?.Trim() ?? "";

            var (pins, lastPage, paginationUrl) = _pinService.GetRecentPinsWithSearch(search, currentPage == 0 ? 1 : currentPage, _perPage);

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }

            ViewBag.SearchString = search;
            ViewBag.Pins = pins;
            ViewBag.lastPage = lastPage;
            ViewBag.PaginationBaseUrl = paginationUrl;

            return View();
        }

        public IActionResult Show(int? id)
        {
            var user = _userManager.GetUserAsync(User).Result;

            var (pin, categories, hasLiked) = _pinService.GetPinDetails(id, user);

            if (pin == null)
            {
                return NotFound();
            }

            ViewBag.Liked = hasLiked;
            ViewBag.Pin = pin;
            ViewBag.Comments = pin.Comments;
            ViewBag.Categories = categories;

            return View();
        }

        [HttpGet]
        public IActionResult GeneralShow(int? id)
        {
            var pin = _pinService.GetPinForGeneralView(id);

            if (pin == null)
            {
                return NotFound();
            }

            ViewBag.Pin = pin;
            ViewBag.Comments = pin.Comments;

            return View();
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> New(Pin pin, IFormFile Path)
        {
            var user = await _userManager.GetUserAsync(User);

            var (isValid, errorMessage) = await _pinService.CreatePinAsync(pin, Path, user, _env);

            if (!isValid)
            {
                return View(pin);
            }

            return RedirectToAction("Show", new { id = pin.Id });
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            string currentUserId = _userManager.GetUserId(User);

            var (pin, isOwner) = _pinService.GetEditablePin(id, currentUserId);

            if (pin == null)
            {
                return NotFound();
            }

            if (!isOwner)
            {
                return Forbid();
            }

            return View(pin);
        }

        [HttpPost]
        public IActionResult Edit(int id, Pin requestPin)
        {
            var currentUserId = _userManager.GetUserId(User);

            try
            {
                bool updated = _pinService.TryUpdatePin(id, requestPin, currentUserId);

                if (!updated)
                {
                    return Forbid();
                }

                return RedirectToAction("Show", new { id = id });
            }
            catch (Exception)
            {
                ViewBag.Pin = requestPin;
                return View(requestPin);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isAdmin = User.IsInRole("Admin");

            bool deleted = _pinService.TryDeletePin(id, currentUser, isAdmin, _env);

            if (!deleted)
            {
                return Forbid();
            }

            return RedirectToAction("Index");
        }



        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult Like(int id)
        {
            var userId = _userManager.GetUserId(User);

            bool updated = _pinService.ToggleLike(id, userId);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction("Show", new { id = id });
        }
    }
}
