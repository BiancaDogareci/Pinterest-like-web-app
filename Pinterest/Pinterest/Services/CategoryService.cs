using Pinterest.Models;
using Pinterest.Repositories;

namespace Pinterest.Services;

public class CategoryService
{
    private readonly CategoryRepository _repo;

    public CategoryService(CategoryRepository repo)
    {
        _repo = repo;
    }

    public List<Category> GetCategoriesForUser(string userId)
    {
        return _repo.GetCategoriesByUserId(userId);
    }

    public (List<Pin> Pins, Category? Category) GetCategoryWithPins(int? categoryId)
    {
        var pins = _repo.GetPinsForCategory(categoryId);
        var category = _repo.GetCategoryById(categoryId);

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
        _repo.SavePinToCategory(categoryId, pinId);
    }

    public bool RemovePinFromCategory(int categoryId, int pinId, string currentUserId, bool isAdmin)
    {
        var pc = _repo.GetPinCategory(categoryId, pinId);
        if (pc == null) return false;

        var category = _repo.GetCategoryById(categoryId);
        if (category == null) return false;

        if (category.AppUserId != currentUserId && !isAdmin)
            return false;

        _repo.RemovePinCategory(pc);
        return true;
    }

    public bool DeleteCategory(int categoryId, string currentUserId, bool isAdmin)
    {
        var category = _repo.GetCategoryById(categoryId);
        if (category == null) return false;

        if (category.AppUserId != currentUserId && !isAdmin)
            return false;

        _repo.RemoveCategory(category);
        return true;
    }
}