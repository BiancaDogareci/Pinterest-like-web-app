using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pinterest.Data;
using Pinterest.Models;

namespace Pinterest.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private IWebHostEnvironment _env;

        private readonly UserManager<AppUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CommentsController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Show(int? id)
        {
            Comment comment = db.Comments.Find(id);
            ViewBag.Comment = comment;

            return View();
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult New(int? pinId)
        {
            ViewBag.PinId = pinId;
               
            return View();
        }
        [HttpPost]
        public IActionResult New(Comment newComment, int pinId)
        {
            newComment.Date = DateTime.Now;
            newComment.PinId = pinId;
            newComment.AppUserId = _userManager.GetUserId(User);
            
            if (ModelState.IsValid)
            {
                db.Comments.Add(newComment);
                db.SaveChanges();
                return RedirectToAction("Show", "Pins", new { id = newComment.PinId });
            }
            else
            {
                ViewBag.PinId = pinId;
                return View(newComment);          
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            Comment comment = db.Comments.Find(id);

            AppUser currentUser = db.AppUsers.Find(_userManager.GetUserId(User));
            if (currentUser.Id != comment.AppUserId)
            {
                return Forbid();
            }

            return View(comment);
        }
        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult Edit(int id, Comment requestComment)
        {
            Comment comment = db.Comments.Find(id);

            try
            {
                comment.Text = requestComment.Text;
                comment.Date = DateTime.Now;
                db.SaveChanges();

                return RedirectToAction("Show", "Pins", new { id = comment.PinId });
            }
            catch (Exception)
            {
                ViewBag.Comment = comment;
                return View(requestComment);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Comment comment = db.Comments.Find(id);

            AppUser currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != comment.AppUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }


            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Show", "Pins", new { id = comment.PinId });
        }
    }
}
