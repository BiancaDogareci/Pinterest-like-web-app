using Microsoft.AspNetCore.Mvc.ModelBinding;
using Pinterest.Models;
using Pinterest.Repositories;
using System.ComponentModel.DataAnnotations;



namespace Pinterest.Services
{
    public class PinService
    {
        private readonly PinRepository _repo;

        public PinService(PinRepository repo)
        {
            _repo = repo;
        }



        public (IEnumerable<Pin> Pins, int LastPage, string PaginationUrl) GetPins(string search, int page, int perPage)
        {
            IQueryable<Pin> pins = string.IsNullOrWhiteSpace(search)
                ? _repo.GetAllPinsOrderedByLikes()
                : _repo.GetPinsBySearch(search);

            int totalItems = pins.Count();

            if (perPage <= 0)
            {
                return (new List<Pin>(), 0, string.IsNullOrWhiteSpace(search)
                    ? "/Pins/Index/?page"
                    : $"/Pins/Index/?search={search}&page");
            }

            int offset = (page - 1) * perPage;

            var paginatedPins = pins.Skip(offset).Take(perPage).ToList();
            int lastPage = (int)Math.Ceiling((float)totalItems / perPage);
            string url = string.IsNullOrWhiteSpace(search)
                ? "/Pins/Index/?page"
                : $"/Pins/Index/?search={search}&page";

            return (paginatedPins, lastPage, url);
        }

        public (IEnumerable<Pin> Pins, int LastPage, string PaginationUrl) GetRecentPinsWithSearch(string search, int page, int perPage)
        {
            IQueryable<Pin> pins = string.IsNullOrWhiteSpace(search)
                ? _repo.GetAllPinsOrderedByDate()
                : _repo.GetPinsBySearch(search);

            int totalItems = pins.Count();

            if (perPage <= 0)
            {
                return (new List<Pin>(), 0, string.IsNullOrWhiteSpace(search)
                    ? "/Pins/IndexRecent/?page"
                    : $"/Pins/IndexRecent/?search={search}&page");
            }

            int offset = (page - 1) * perPage;

            var paginatedPins = pins.Skip(offset).Take(perPage).ToList();
            int lastPage = (int)Math.Ceiling((float)totalItems / perPage);
            string url = string.IsNullOrWhiteSpace(search)
                ? "/Pins/IndexRecent/?page"
                : $"/Pins/IndexRecent/?search={search}&page";

            return (paginatedPins, lastPage, url);
        }
        
        public (Pin? Pin, List<Category> Categories, bool HasLiked) GetPinDetails(int? pinId, AppUser user)
        {
            var pin = _repo.GetPinWithCommentsAndAuthors(pinId);

            if (pin == null)
                return (null, new List<Category>(), false);

            var categories = _repo.GetAvailableCategoriesForUser(user.Id, pinId);
            var hasLiked = _repo.HasUserLikedPin(pinId.Value, user.Id);

            return (pin, categories, hasLiked);
        }

        public Pin? GetPinForGeneralView(int? id)
        {
            return _repo.GetPinWithCommentsAndAuthors(id);
        }

        public async Task<(bool IsValid, string? ErrorMessage)> CreatePinAsync(Pin pin, IFormFile file, AppUser user, IWebHostEnvironment env)
        {
            if (file != null && file.Length > 0)
            {
                string folder;

                if (file.ContentType.Contains("image"))
                {
                    folder = "images";
                }
                else if (file.ContentType.Contains("video"))
                {
                    folder = "videos";
                }
                else
                {
                    return (false, "Unsupported media type");
                }

                var fileName = file.FileName;
                var dbPath = $"/{folder}/{fileName}";
                var physicalPath = Path.Combine(env.WebRootPath, folder, fileName);

                // ne asiguram ca folderul exista
                Directory.CreateDirectory(Path.GetDirectoryName(physicalPath));

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                pin.EmbeddedContentPath = dbPath;
            }

            pin.Date = DateTime.Now;
            pin.LikesCount = 0;
            pin.AppUserId = user.Id;

            // Resetam si validam manual
            var context = new ValidationContext(pin);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(pin, context, results, true);

            if (!isValid)
            {
                return (false, "Model validation failed.");
            }

            _repo.AddPin(pin);
            return (true, null);
        }

        public (Pin? Pin, bool IsOwner) GetEditablePin(int? pinId, string currentUserId)
        {
            var pin = _repo.GetPinById(pinId);
            if (pin == null)
            {
                return (null, false);
            }

            bool isOwner = pin.AppUserId == currentUserId;
            return (pin, isOwner);
        }

        public bool TryUpdatePin(int id, Pin updatedPin, string userId)
        {
            var pin = _repo.GetPinById(id);

            if (pin == null || pin.AppUserId != userId)
            {
                return false;
            }

            pin.Title = updatedPin.Title;
            pin.Description = updatedPin.Description;
            pin.Date = DateTime.Now;

            _repo.UpdatePin(pin);
            return true;
        }

        public bool TryDeletePin(int? pinId, AppUser currentUser, bool isAdmin, IWebHostEnvironment env)
        {
            var pin = _repo.GetPinById(pinId);
            if (pin == null)
                return false;

            if (pin.AppUserId != currentUser.Id && !isAdmin)
                return false;

            string filePath = Path.Combine(env.WebRootPath, pin.EmbeddedContentPath.TrimStart('/'));

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _repo.DeletePin(pin);
            return true;
        }

        public bool ToggleLike(int pinId, string userId)
        {
            var pin = _repo.GetPinById(pinId);
            if (pin == null) return false;

            bool hasLiked = _repo.HasUserLikedPin(pinId, userId);

            if (hasLiked)
            {
                pin.LikesCount--;
                _repo.RemoveLike(pinId, userId);
            }
            else
            {
                pin.LikesCount++;
                _repo.AddLike(pinId, userId);
            }

            _repo.Save();
            return true;
        }

    }
}
