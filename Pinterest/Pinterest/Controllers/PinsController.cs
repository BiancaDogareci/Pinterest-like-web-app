using Pinterest.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Pinterest.Data;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Pinterest.Controllers
{
    public class PinsController : Controller
    {
        private readonly ApplicationDbContext db;
        private IWebHostEnvironment _env;

        private readonly UserManager<AppUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public PinsController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int _perPage = 3;

            var pins = from pin in db.Pins
                       orderby pin.LikesCount descending
                       select pin;

            var search = "";

            // MOTOR DE CAUTARE
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                // Eliminam spatiile libere
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                // Cautare in pin (Titlu si Descriere)
                List<int> pinIds = db.Pins.Where
                (
                at => at.Title.Contains(search)
                || at.Description.Contains(search)
                ).Select(a => a.Id).ToList();

                // Cautare in comentarii
                List<int> pinIdsOfCommentsWithSearchString =
                db.Comments.Where
                (
                c => c.Text.Contains(search)
                ).Select(c => (int)c.PinId).ToList();

                // Se formeaza o singura lista formata din toate id-urile selectate anterior
                List<int> mergedIds = pinIds.Union(pinIdsOfCommentsWithSearchString).ToList();
                
                pins = db.Pins.Where(pin =>
                mergedIds.Contains(pin.Id))
                .Include("AppUser")
                .OrderBy(a => a.Title);
            }
            ViewBag.SearchString = search;

            // Afisare paginata
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }

            int totalItems = pins.Count();
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            var paginatedPins = pins.Skip(offset).Take(_perPage);

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            ViewBag.Pins = paginatedPins;


            // MOTOR DE CAUTARE (continuare)
            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Pins/Index/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Pins/Index/?page";
            }

            return View();
        }

        [HttpGet]
        public IActionResult IndexRecent()
        {
            int _perPage = 3;

            var pins = from pin in db.Pins
                       orderby pin.Date descending
                       select pin;

            var search = "";
            // MOTOR DE CAUTARE
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                // Eliminam spatiile libere
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                // Cautare in pin (Titlu si Descriere)
                List<int> pinIds = db.Pins.Where
                (
                at => at.Title.Contains(search)
                || at.Description.Contains(search)
                ).Select(a => a.Id).ToList();

                // Cautare in comentarii
                List<int> pinIdsOfCommentsWithSearchString =
                db.Comments.Where
                (
                c => c.Text.Contains(search)
                ).Select(c => (int)c.PinId).ToList();

                // Se formeaza o singura lista formata din toate id-urile selectate anterior
                List<int> mergedIds = pinIds.Union(pinIdsOfCommentsWithSearchString).ToList();
                pins = db.Pins.Where(pin =>
                mergedIds.Contains(pin.Id))
                .Include("AppUser")
                .OrderBy(a => a.Title);
            }
            ViewBag.SearchString = search;


            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }

            int totalItems = pins.Count();

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            var paginatedPins = pins.Skip(offset).Take(_perPage);

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            ViewBag.Pins = paginatedPins;

            // MOTOR DE CAUTARE (continuare)
            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Pins/Index/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Pins/Index/?page";
            }

            return View();
        }

        public IActionResult Show(int? id)
        {
            var pin = db.Pins
                .Include(p => p.Comments)
                .ThenInclude(c => c.AppUser)
                .FirstOrDefault(p => p.Id == id);

            var user = _userManager.GetUserAsync(User).Result;

            var categories = from c in db.Categories
                             where c.AppUserId == user.Id
                             && !(from pc in db.PinCategories
                                  where pc.PinId == id
                                  select pc.CategoryId).Contains(c.Id)
                             select c;

            bool hasLiked = db.Likes.Any(like => like.PinId == id && like.AppUserId == user.Id);

            if (pin == null)
            {
                return NotFound();
            }

            ViewBag.Liked = hasLiked;

            ViewBag.Pin = pin;
            ViewBag.Comments = pin.Comments;
            ViewBag.Categories = categories.ToList();

            return View();
        }

        [HttpGet]
        public IActionResult GeneralShow(int? id)
        {
            var pin = db.Pins
                .Include(p => p.Comments)
                .ThenInclude(c => c.AppUser)
                .FirstOrDefault(p => p.Id == id);

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
            AppUser currentUser = await _userManager.GetUserAsync(User);

            // Verificam daca exista imaginea in request (daca a fost incarcata o imagine)
            if (Path != null && Path.Length > 0)
            {
                string folder;

                if (Path.ContentType.Contains("image"))
                {
                    folder = "images";
                }
                else if (Path.ContentType.Contains("video"))
                {
                    folder = "videos";
                }
                else
                {
                    return View(pin);
                }

                var databaseFileName = $"/{folder}/";

                // File upload
                var storagePath = System.IO.Path.Combine(_env.WebRootPath, folder, Path.FileName);
                databaseFileName += Path.FileName;

                
                using (var fileStream = new FileStream(storagePath, FileMode.Create))
                {
                    await Path.CopyToAsync(fileStream);
                }

                pin.EmbeddedContentPath = databaseFileName;
            }

            pin.Date = DateTime.Now;
            pin.LikesCount = 0;
            pin.AppUserId = currentUser.Id;

            // sterge erorile existente
            ModelState.Clear();
            // verifica iar
            TryValidateModel(pin);

            if (!ModelState.IsValid)
            {
                return View(pin);
            }

            db.Pins.Add(pin);
            db.SaveChanges();
            return RedirectToAction("Show", new { id = pin.Id });
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            Pin pin = db.Pins.Find(id);

            AppUser currentUser = db.AppUsers.Find(_userManager.GetUserId(User));
            if (currentUser.Id != pin.AppUserId)
            {
                return Forbid();
            }

            return View(pin);
        }

        [HttpPost]
        public ActionResult Edit(int id, Pin requestPin)
        {
            Pin pin = db.Pins.Find(id);

            try
            {
                pin.Title = requestPin.Title;
                pin.Description = requestPin.Description;
                pin.Date = DateTime.Now;
                db.SaveChanges();

                return RedirectToAction("Show", new { id = pin.Id });
            }
            catch (Exception)
            {
                ViewBag.Pin = pin;
                return View(requestPin);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            Pin pin = db.Pins.Find(id);

            AppUser currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != pin.AppUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            string filePath = Path.Combine(_env.WebRootPath, pin.EmbeddedContentPath.TrimStart('/'));

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            db.Pins.Remove(pin);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult Like(int id)
        {
            Pin pin = db.Pins.Find(id);

            if (pin == null)
            {
                return NotFound();
            }

            AppUser currentUser = _userManager.GetUserAsync(User).Result;

            // Verifica daca userul a dat like respectivului pin
            bool hasLiked = db.Likes.Any(ul => ul.PinId == pin.Id && ul.AppUserId == currentUser.Id);

            if (hasLiked)
            {
                // Daca userul a dat like la acest pin:
                //  - se scade likescount cu 1
                pin.LikesCount--;
                //  - se sterge like-ul din tabela asociativa
                UnlikePin(pin.Id, currentUser.Id);

                return RedirectToAction("Show", new { id = pin.Id });
            }

            // Daca userul nu a dat like la acest pin:
            //  - se creste likescount cu 1
            pin.LikesCount++;
            //  - se adauga like-ul in tabela asociativa
            RecordUserLike(pin.Id);

            return RedirectToAction("Show", new { id = pin.Id });
        }

        private void UnlikePin(int pinId, string userId)
        {
            // Gasesti like-ul respectiv
            Like userLike = db.Likes.FirstOrDefault(ul => ul.PinId == pinId && ul.AppUserId == userId);

            if (userLike != null)
            {
                // Stergi like-ul
                db.Likes.Remove(userLike);
                db.SaveChanges();
            }
        }

        /*
        private async Task<bool> HasUserLikedPin(int pinId)
        {
            AppUser currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return false;
            }

            // Verifica daca user-ul a dat like la pin sau nu
            return db.Likes.Where(ul => ul.PinId == pinId && ul.AppUserId == currentUser.Id).Any();
        }
        */

        private void RecordUserLike(int pinId)
        {
            AppUser currentUser = _userManager.GetUserAsync(User).Result;

            // Adaugam like-ul in baza de date
            Like userLike = new Like
            {
                PinId = pinId,
                AppUserId = currentUser.Id
            };

            db.Likes.Add(userLike);
            db.SaveChanges();
        }
    }
}
