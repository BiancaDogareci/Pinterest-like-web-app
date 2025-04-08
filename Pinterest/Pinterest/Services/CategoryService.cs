using Pinterest.Models;
using Pinterest.Repositories;

namespace Pinterest.Services;

public class CategoryService
{
    private readonly CategoryRepository _repo;
    private readonly PinRepository _pinRepo;
    
    public CategoryService(CategoryRepository repo, PinRepository pinRepo)
    {
        _repo = repo;
        _pinRepo = pinRepo;
    }
    
    public CategoryService(CategoryRepository repo)
    {
        _repo = repo;
        _pinRepo = null!;
    }
    
    public List<Category> GetCategoriesForUser(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty or null.", nameof(userId));
        
        return _repo.GetCategoriesByUserId(userId);
    }
    
    public (List<Pin> Pins, Category? Category) GetCategoryWithPins(int? categoryId)
    {
        if (categoryId == null)
            throw new ArgumentException("Category ID cannot be null.", nameof(categoryId));
        
        var category = _repo.GetCategoryById(categoryId);
        if (category == null)
            throw new InvalidOperationException("Category not found.");
        
        var pins = _repo.GetPinsForCategory(categoryId);

        return (pins, category);
    }

    public void AddCategory(Category category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        _repo.AddCategory(category);
    }

    public void SavePinToCategory(int categoryId, int pinId)
    {
        var category = _repo.GetCategoryById(categoryId);
        var pin = _pinRepo.GetPinById(pinId);

        if (category == null)
            throw new InvalidOperationException("Category does not exist.");
    
        if (pin == null)
            throw new InvalidOperationException("Pin does not exist.");

        _repo.SavePinToCategory(categoryId, pinId);
    }
    
    public bool RemovePinFromCategory(int categoryId, int pinId, string currentUserId, bool isAdmin)
    {
        if (string.IsNullOrWhiteSpace(currentUserId))
            throw new ArgumentException("User ID cannot be null or empty.", nameof(currentUserId));

        var category = _repo.GetCategoryById(categoryId);
        if (category == null)
            throw new InvalidOperationException("Category does not exist.");

        var pc = _repo.GetPinCategory(categoryId, pinId);
        if (pc == null)
            return false;

        if (category.AppUserId != currentUserId && !isAdmin)
            return false;

        _repo.RemovePinCategory(pc);
        return true;
    }

    public bool DeleteCategory(int categoryId, string currentUserId, bool isAdmin)
    {
        if (string.IsNullOrWhiteSpace(currentUserId))
            throw new ArgumentException("User ID cannot be null or empty.", nameof(currentUserId));

        var category = _repo.GetCategoryById(categoryId);
        if (category == null)
            throw new InvalidOperationException("Category not found.");

        if (category.AppUserId != currentUserId && !isAdmin)
            return false;

        _repo.RemoveCategory(category);
        return true;
    }
}