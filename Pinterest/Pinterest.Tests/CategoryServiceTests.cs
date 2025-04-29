using Moq;
using Pinterest.Models;
using Pinterest.Repositories;
using Pinterest.Services;

namespace Pinterest.Tests;

public class CategoryServiceTests
{
    // GetCategoriesForUser
    
    // userId
    // "user123" (valid) - t1
    // "" (invalid) - t2
    // null (invalid) - t3
    
    [Fact]
    public void GetCategoriesForUser_ValidUserId_ReturnsCategories_t1()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        var expectedCategories = new List<Category>
        {
            new Category { Id = 1, Name = "Work" },
            new Category { Id = 2, Name = "Travel" }
        };

        repoMock.Setup(r => r.GetCategoriesByUserId("user123")).Returns(expectedCategories);
        var service = new CategoryService(repoMock.Object);

        var result = service.GetCategoriesForUser("user123");

        Assert.Equal(2, result.Count);
        Assert.Equal("Work", result[0].Name);
        repoMock.Verify(r => r.GetCategoriesByUserId("user123"), Times.Once);
    }

    [Fact]
    public void GetCategoriesForUser_UserIdIsEmpty_ThrowsArgumentException_t2()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        var service = new CategoryService(repoMock.Object);

        var ex = Assert.Throws<ArgumentException>(() => service.GetCategoriesForUser(""));
        Assert.Equal("User ID cannot be empty or null. (Parameter 'userId')", ex.Message);
    }

    [Fact]
    public void GetCategoriesForUser_UserIdIsNull_ThrowsArgumentException_t3()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        var service = new CategoryService(repoMock.Object);

        var ex = Assert.Throws<ArgumentException>(() => service.GetCategoriesForUser(null));
        Assert.Equal("User ID cannot be empty or null. (Parameter 'userId')", ex.Message);
    }
    
    
    // GetCategoryWithPins
    
    // categoryId
    // exista in bd (valid) - t1
    // null (invalid) - t2
    // nu exista in db (invalid) - t3

    [Fact]
    public void GetCategoryWithPins_ValidCategoryId_ReturnsPinsAndCategory_t1()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        var categoryId = 1;

        var category = new Category { Id = categoryId, Name = "Inspiration" };
        var pins = new List<Pin>
        {
            new Pin { Id = 1, Title = "Pin 1" },
            new Pin { Id = 2, Title = "Pin 2" }
        };

        repoMock.Setup(r => r.GetCategoryById(categoryId)).Returns(category);
        repoMock.Setup(r => r.GetPinsForCategory(categoryId)).Returns(pins);

        var service = new CategoryService(repoMock.Object);

        var result = service.GetCategoryWithPins(categoryId);

        Assert.Equal(2, result.Pins.Count);
        Assert.Equal(category, result.Category);
    }

    [Fact]
    public void GetCategoryWithPins_NullCategoryId_ThrowsArgumentException_t2()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        var service = new CategoryService(repoMock.Object);

        var exception = Assert.Throws<ArgumentException>(() => service.GetCategoryWithPins(null));
        Assert.Equal("Category ID cannot be null. (Parameter 'categoryId')", exception.Message);
    }

    [Fact]
    public void GetCategoryWithPins_CategoryDoesNotExist_ThrowsInvalidOperationException_t3()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        int categoryId = 99;

        repoMock.Setup(r => r.GetCategoryById(categoryId)).Returns((Category)null);

        var service = new CategoryService(repoMock.Object);

        var exception = Assert.Throws<InvalidOperationException>(() => service.GetCategoryWithPins(categoryId));
        Assert.Equal("Category not found.", exception.Message);
    }

    
    // AddCategory
    
    // category
    // obiect valid (valid) - t1
    // null (invalid) - t2

    [Fact]
    public void AddCategory_ValidCategory_AddsSuccessfully_t1()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        var service = new CategoryService(repoMock.Object);

        var category = new Category { Id = 1, Name = "New Category", AppUserId = "user1" };

        service.AddCategory(category);

        repoMock.Verify(r => r.AddCategory(category), Times.Once);
    }

    [Fact]
    public void AddCategory_NullCategory_ThrowsArgumentNullException_t2()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        var service = new CategoryService(repoMock.Object);

        var ex = Assert.Throws<ArgumentNullException>(() => service.AddCategory(null));
        Assert.Equal("category", ex.ParamName);
    }
    
    
    // SavePinToCategory
    
    // categoryId
    // exista in bd (valid) - t1
    // nu exista in db (invalid) - t2
    
    // pinId
    // exista in bd (valid) - t1
    // nu exista in db (invalid) - t3
    
    [Fact]
    public void SavePinToCategory_ValidInputs_CallsRepoMethod_t1()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        var pinRepoMock = new Mock<PinRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(new Category { Id = 1 });
        pinRepoMock.Setup(r => r.GetPinById(10)).Returns(new Pin { Id = 10 });

        var service = new CategoryService(repoMock.Object, pinRepoMock.Object);

        service.SavePinToCategory(1, 10);

        repoMock.Verify(r => r.SavePinToCategory(1, 10), Times.Once);
    }
    
    [Fact]
    public void SavePinToCategory_NonexistentCategory_ThrowsException_t2()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        var pinRepoMock = new Mock<PinRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns((Category)null);
        pinRepoMock.Setup(r => r.GetPinById(10)).Returns(new Pin { Id = 10 });

        var service = new CategoryService(repoMock.Object, pinRepoMock.Object);

        var ex = Assert.Throws<InvalidOperationException>(() => service.SavePinToCategory(1, 10));
        Assert.Equal("Category does not exist.", ex.Message);
    }

    [Fact]
    public void SavePinToCategory_NonexistentPin_ThrowsException_t3()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        var pinRepoMock = new Mock<PinRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(new Category { Id = 1 });
        pinRepoMock.Setup(r => r.GetPinById(10)).Returns((Pin)null);

        var service = new CategoryService(repoMock.Object, pinRepoMock.Object);

        Assert.Throws<InvalidOperationException>(() => service.SavePinToCategory(1, 10));
    }

    
    // RemovePinFromCategory
    
    // categoryId
    // exista in db (valid) - t1
    // nu exista in db (invalid) - t2
    
    // pinId
    // e asociat categoriei (valid) - t1
    // nu e asociat categoriei (invalid) - t3
    
    // currentUserId + isAdmin (true/false)
    // utilizatorul e owner (valid) - t1
    // utilizatorul nu e owner, dar e admin (isAdmin = true) (valid) - t4
    // utilizatorul nu e owner, nici admin (isAdmin = false) (invalid) - t5
    // null (invalid) - t6
    // "' (invalid) - t7
    
    [Fact]
    public void RemovePinFromCategory_ValidInputsUserIsOwner_RemovesAndReturnsTrue_t1()
    {
        var pc = new PinCategory { CategoryId = 1, PinId = 10 };
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(new Category { Id = 1, AppUserId = "user123" });
        repoMock.Setup(r => r.GetPinCategory(1, 10)).Returns(pc);

        var service = new CategoryService(repoMock.Object);

        var result = service.RemovePinFromCategory(1, 10, "user123", false);

        Assert.True(result);
        repoMock.Verify(r => r.RemovePinCategory(pc), Times.Once);
    }

    [Fact]
    public void RemovePinFromCategory_CategoryDoesNotExist_ThrowsException_t2()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns((Category)null);

        var service = new CategoryService(repoMock.Object);

        Assert.Throws<InvalidOperationException>(() => service.RemovePinFromCategory(1, 10, "user123", false));
    }
    
    [Fact]
    public void RemovePinFromCategory_PinCategoryDoesNotExist_ReturnsFalse_t3()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(new Category { Id = 1, AppUserId = "user123" });
        repoMock.Setup(r => r.GetPinCategory(1, 10)).Returns((PinCategory)null);

        var service = new CategoryService(repoMock.Object);

        var result = service.RemovePinFromCategory(1, 10, "user123", false);

        Assert.False(result);
    }
    
    [Fact]
    public void RemovePinFromCategory_ValidInputsUserIsAdmin_RemovesAndReturnsTrue_t4()
    {
        var pc = new PinCategory { CategoryId = 1, PinId = 10 };
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(new Category { Id = 1, AppUserId = "owner" });
        repoMock.Setup(r => r.GetPinCategory(1, 10)).Returns(pc);

        var service = new CategoryService(repoMock.Object);

        var result = service.RemovePinFromCategory(1, 10, "adminGuy", true);

        Assert.True(result);
        repoMock.Verify(r => r.RemovePinCategory(pc), Times.Once);
    }

    [Fact]
    public void RemovePinFromCategory_UserNotOwnerOrAdmin_ReturnsFalse_t5()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(new Category { Id = 1, AppUserId = "owner" });
        repoMock.Setup(r => r.GetPinCategory(1, 10)).Returns(new PinCategory { CategoryId = 1, PinId = 10 });

        var service = new CategoryService(repoMock.Object);

        var result = service.RemovePinFromCategory(1, 10, "intruder", false);

        Assert.False(result);
    }
    
    [Fact]
    public void RemovePinFromCategory_NullUserId_ThrowsException_t6()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(new Category { Id = 1, AppUserId = "user123" });
        repoMock.Setup(r => r.GetPinCategory(1, 10)).Returns(new PinCategory { CategoryId = 1, PinId = 10 });

        var service = new CategoryService(repoMock.Object);

        Assert.Throws<ArgumentException>(() => service.RemovePinFromCategory(1, 10, null, false));
    }

    [Fact]
    public void RemovePinFromCategory_EmptyUserId_ThrowsException_t7()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(new Category { Id = 1, AppUserId = "user123" });
        repoMock.Setup(r => r.GetPinCategory(1, 10)).Returns(new PinCategory { CategoryId = 1, PinId = 10 });

        var service = new CategoryService(repoMock.Object);

        Assert.Throws<ArgumentException>(() => service.RemovePinFromCategory(1, 10, "", false));
    }
    
    
    // DeleteCategory
    
    // categoryId
    // exista in db (valid) - t1
    // nu exista in db (invalid) - t2
    
    // currentUserId + isAdmin (true/false)
    // utilizatorul e owner (valid) - t1
    // utilizatorul nu e owner, dar e admin (isAdmin = true) (valid) - t3
    // utilizatorul nu e owner, nici admin (isAdmin = false) (invalid) - t4
    // "" (invalid) - t5
    // null (invalid) - t6
    
    [Fact]
    public void DeleteCategory_ValidInputsUserIsOwner_DeletesAndReturnsTrue_t1()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        var category = new Category { Id = 1, AppUserId = "user123" };
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(category);

        var service = new CategoryService(repoMock.Object);

        var result = service.DeleteCategory(1, "user123", false);

        Assert.True(result);
        repoMock.Verify(r => r.RemoveCategory(category), Times.Once);
    }
    
    [Fact]
    public void DeleteCategory_CategoryDoesNotExist_ThrowsException_t2()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns((Category)null);

        var service = new CategoryService(repoMock.Object);

        Assert.Throws<InvalidOperationException>(() => service.DeleteCategory(1, "user123", false));
    }

    [Fact]
    public void DeleteCategory_ValidInputsUserIsAdmin_DeletesAndReturnsTrue_t3()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        var category = new Category { Id = 1, AppUserId = "user123" };
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(category);

        var service = new CategoryService(repoMock.Object);

        var result = service.DeleteCategory(1, "adminUser", true);

        Assert.True(result);
        repoMock.Verify(r => r.RemoveCategory(category), Times.Once);
    }

    [Fact]
    public void DeleteCategory_InvalidUserNotAdmin_ReturnsFalse_t4()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        var category = new Category { Id = 1, AppUserId = "user123" };
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(category);

        var service = new CategoryService(repoMock.Object);

        var result = service.DeleteCategory(1, "anotherUser", false);

        Assert.False(result);
        repoMock.Verify(r => r.RemoveCategory(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public void DeleteCategory_EmptyUserId_ThrowsException_t5()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(new Category { Id = 1, AppUserId = "user123" });

        var service = new CategoryService(repoMock.Object);

        Assert.Throws<ArgumentException>(() => service.DeleteCategory(1, "", false));
    }

    [Fact]
    public void DeleteCategory_NullUserId_ThrowsException_t6()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(new Category { Id = 1, AppUserId = "user123" });

        var service = new CategoryService(repoMock.Object);

        Assert.Throws<ArgumentException>(() => service.DeleteCategory(1, null, false));
    }
}