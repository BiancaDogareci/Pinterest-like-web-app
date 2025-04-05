using Pinterest.Models;
using Pinterest.Data;
using Microsoft.EntityFrameworkCore;



namespace Pinterest.Repositories
{
    public class PinRepository
    {
        private readonly ApplicationDbContext _db;

        public PinRepository(ApplicationDbContext db)
        {
            _db = db;
        }



        public IQueryable<Pin> GetAllPinsOrderedByLikes()
        {
            return _db.Pins.OrderByDescending(p => p.LikesCount);
        }

        public IQueryable<Pin> GetPinsBySearch(string search)
        {
            var pinIds = _db.Pins
                .Where(p => p.Title.Contains(search) || p.Description.Contains(search))
                .Select(p => p.Id);

            var commentPinIds = _db.Comments
                .Where(c => c.Text.Contains(search))
                .Select(c => c.PinId.Value);

            var mergedIds = pinIds.Union(commentPinIds);

            return _db.Pins
                .Where(p => mergedIds.Contains(p.Id))
                .Include(p => p.AppUser)
                .OrderBy(p => p.Title);
        }

        public IQueryable<Pin> GetAllPinsOrderedByDate()
        {
            return _db.Pins.OrderByDescending(p => p.Date);
        }
        public Pin? GetPinWithCommentsAndAuthors(int? id)
        {
            return _db.Pins
                .Include(p => p.Comments)
                .ThenInclude(c => c.AppUser)
                .FirstOrDefault(p => p.Id == id);
        }

        public List<Category> GetAvailableCategoriesForUser(string userId, int? pinId)
        {
            var usedCategoryIds = _db.PinCategories
                .Where(pc => pc.PinId == pinId)
                .Select(pc => pc.CategoryId);

            return _db.Categories
                .Where(c => c.AppUserId == userId && !usedCategoryIds.Contains(c.Id))
                .ToList();
        }

        public bool HasUserLikedPin(int pinId, string userId)
        {
            return _db.Likes.Any(like => like.PinId == pinId && like.AppUserId == userId);
        }

        public void AddPin(Pin pin)
        {
            _db.Pins.Add(pin);
            _db.SaveChanges();
        }

        public Pin? GetPinById(int? id)
        {
            return _db.Pins.Find(id);
        }

        public AppUser? GetAppUserById(string userId)
        {
            return _db.AppUsers.Find(userId);
        }

        public void UpdatePin(Pin pin)
        {
            _db.SaveChanges();
        }

        public void DeletePin(Pin pin)
        {
            _db.Pins.Remove(pin);
            _db.SaveChanges();
        }

        public void AddLike(int pinId, string userId)
        {
            var like = new Like { PinId = pinId, AppUserId = userId };
            _db.Likes.Add(like);
            _db.SaveChanges();
        }

        public void RemoveLike(int pinId, string userId)
        {
            var like = _db.Likes.FirstOrDefault(l => l.PinId == pinId && l.AppUserId == userId);
            if (like != null)
            {
                _db.Likes.Remove(like);
                _db.SaveChanges();
            }
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
