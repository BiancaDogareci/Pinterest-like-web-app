using Pinterest.Data;
using Pinterest.Models;
using Microsoft.EntityFrameworkCore;

namespace Pinterest.Repositories;

public class CategoryRepository
{
    private readonly ApplicationDbContext _db;

    public CategoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<Category> GetCategoriesByUserId(string userId)
    {
        return _db.Categories
            .Where(c => c.AppUserId == userId)
            .ToList();
    }

    public Category? GetCategoryById(int? categoryId)
    {
        return _db.Categories.Find(categoryId);
    }

    public List<Pin> GetPinsForCategory(int? categoryId)
    {
        return _db.PinCategories
            .Where(pc => pc.CategoryId == categoryId)
            .Include(pc => pc.Pin)
            .Select(pc => pc.Pin!)
            .ToList();
    }

    public void AddCategory(Category category)
    {
        _db.Categories.Add(category);
        _db.SaveChanges();
    }

    public void SavePinToCategory(int categoryId, int pinId)
    {
        var pc = new PinCategory
        {
            CategoryId = categoryId,
            PinId = pinId,
            CategoryDate = DateTime.Now
        };

        _db.PinCategories.Add(pc);
        _db.SaveChanges();
    }

    public PinCategory? GetPinCategory(int categoryId, int pinId)
    {
        return _db.PinCategories
            .FirstOrDefault(pc => pc.CategoryId == categoryId && pc.PinId == pinId);
    }

    public void RemovePinCategory(PinCategory pc)
    {
        _db.PinCategories.Remove(pc);
        _db.SaveChanges();
    }

    public void RemoveCategory(Category category)
    {
        _db.Categories.Remove(category);
        _db.SaveChanges();
    }
}