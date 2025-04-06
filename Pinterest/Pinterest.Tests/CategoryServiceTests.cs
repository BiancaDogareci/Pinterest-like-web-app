using Moq;
using Pinterest.Models;
using Pinterest.Repositories;
using Pinterest.Services;

namespace Pinterest.Tests;

public class CategoryServiceTests
{
    // GetCategoriesForUser
    // 1. user id valid, returneaza lista valida -> clasa de echivalenta
    // 2. user id este "", operatie invalida -> analiza de frontiera
    // 3. user id este null, operatie invalida -> analiza de frontiera (null)
    
    [Fact]
    public void GetCategoriesForUser_ValidUserId_ReturnsCategories()
    {
        // Arrange
        var repoMock = new Mock<CategoryRepository>(null);
        var expectedCategories = new List<Category>
        {
            new Category { Id = 1, Name = "Work" },
            new Category { Id = 2, Name = "Travel" }
        };

        repoMock.Setup(r => r.GetCategoriesByUserId("user123")).Returns(expectedCategories);
        var service = new CategoryService(repoMock.Object);

        // Act
        var result = service.GetCategoriesForUser("user123");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Work", result[0].Name);
        repoMock.Verify(r => r.GetCategoriesByUserId("user123"), Times.Once);
    }

    [Fact]
    public void GetCategoriesForUser_EmptyUserId_ReturnsEmptyList()
    {
        // Arrange
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoriesByUserId("")).Returns(new List<Category>());

        var service = new CategoryService(repoMock.Object);

        // Act
        var result = service.GetCategoriesForUser("");

        // Assert
        Assert.Empty(result);
        repoMock.Verify(r => r.GetCategoriesByUserId(""), Times.Once);
    }

    [Fact]
    public void GetCategoriesForUser_NullUserId_ReturnsEmptyList()
    {
        // Arrange
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoriesByUserId(null)).Returns(new List<Category>());

        var service = new CategoryService(repoMock.Object);

        // Act
        var result = service.GetCategoriesForUser(null);

        // Assert
        Assert.Empty(result);
        repoMock.Verify(r => r.GetCategoriesByUserId(null), Times.Once);
    }
    
    
    // GetCategoryWithPins
    // 1. category id valid, returneaza pins + category -> clasa de echivalenta
    // 2. category id e valid, dar categoria nu exista in baza de date, operatie invalida -> clasa de frontiera
    // 3. category id este null, operatia invalida -> analiza de frontiera

    [Fact]
    public void GetCategoryWithPins_ValidId_ReturnsData()
    {
        // Arrange
        var repoMock = new Mock<CategoryRepository>(null);
        var pins = new List<Pin> { new Pin { Id = 1 }, new Pin { Id = 2 } };
        var category = new Category { Id = 1, Name = "Nature" };

        repoMock.Setup(r => r.GetPinsForCategory(1)).Returns(pins);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(category);

        var service = new CategoryService(repoMock.Object);

        // Act
        var result = service.GetCategoryWithPins(1);

        // Assert
        Assert.Equal(2, result.Pins.Count);
        Assert.Equal("Nature", result.Category.Name);
    }

    [Fact]
    public void GetCategoryWithPins_CategoryNotFound_ReturnsNullCategory()
    {
        // Arrange
        var repoMock = new Mock<CategoryRepository>(null);
        var pins = new List<Pin> { new Pin { Id = 1 } };

        repoMock.Setup(r => r.GetPinsForCategory(99)).Returns(pins);
        repoMock.Setup(r => r.GetCategoryById(99)).Returns((Category)null);

        var service = new CategoryService(repoMock.Object);

        // Act
        var result = service.GetCategoryWithPins(99);

        // Assert
        Assert.Equal(pins, result.Pins);
        Assert.Null(result.Category);
    }

    [Fact]
    public void GetCategoryWithPins_NullCategoryId_ReturnsEmptyPinsAndNullCategory()
    {
        // Arrange
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetPinsForCategory(null)).Returns(new List<Pin>());
        repoMock.Setup(r => r.GetCategoryById(null)).Returns((Category)null);

        var service = new CategoryService(repoMock.Object);

        // Act
        var result = service.GetCategoryWithPins(null);

        // Assert
        Assert.Empty(result.Pins);
        Assert.Null(result.Category);
    }

    
    // AddCategory
    // 1. categoria este valida, operatie valida -> clasa de echivalenta
    // 2. obiectul trimis e null, operatie invalida -> analiza de frontiera

    [Fact]
    public void AddCategory_ValidCategory_CallsRepositoryAdd()
    {
        // Arrange
        var repoMock = new Mock<CategoryRepository>(null);
        var category = new Category { Id = 1, Name = "Home" };

        var service = new CategoryService(repoMock.Object);

        // Act
        service.AddCategory(category);

        // Assert
        repoMock.Verify(r => r.AddCategory(category), Times.Once);
    }

    [Fact]
    public void AddCategory_NullCategory_ThrowsException()
    {
        // Arrange
        var repoMock = new Mock<CategoryRepository>(null);
        var service = new CategoryService(repoMock.Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.AddCategory(null));
    }
    
    
    // SavePinToCategory
    // 1.
    // 2.
    
    [Fact]
    public void SavePinToCategory_ValidData_CallsRepositoryMethod()
    {
        // Arrange
        var repoMock = new Mock<CategoryRepository>(null);
        var service = new CategoryService(repoMock.Object);

        int categoryId = 10;
        int pinId = 42;

        // Act
        service.SavePinToCategory(categoryId, pinId);

        // Assert
        repoMock.Verify(r => r.SavePinToCategory(categoryId, pinId), Times.Once);
    }

    
    // RemovePinFromCategory
    // 1. pin category id ul nu exista -> clasa de echivalenta
    // 2. categoria nu exista -> clasa de echivalenta
    // 3. user ul nu e owner si nici admin -> clasa de echivalenta
    // 4. user ul e owner -> clasa de echivalenta
    // 5. user ul nu e owner, dar e admin -> clasa de echivalenta
    
    [Fact]
    public void RemovePinFromCategory_PinCategoryNotFound_ReturnsFalse()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetPinCategory(1, 2)).Returns((PinCategory)null);

        var service = new CategoryService(repoMock.Object);

        var result = service.RemovePinFromCategory(1, 2, "user", false);

        Assert.False(result);
    }

    [Fact]
    public void RemovePinFromCategory_CategoryNotFound_ReturnsFalse()
    {
        var pc = new PinCategory();
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetPinCategory(1, 2)).Returns(pc);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns((Category)null);

        var service = new CategoryService(repoMock.Object);

        var result = service.RemovePinFromCategory(1, 2, "user", false);

        Assert.False(result);
    }
    
    [Fact]
    public void RemovePinFromCategory_UserNotOwnerAndNotAdmin_ReturnsFalse()
    {
        var pc = new PinCategory();
        var category = new Category { AppUserId = "owner" };

        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetPinCategory(1, 2)).Returns(pc);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(category);

        var service = new CategoryService(repoMock.Object);

        var result = service.RemovePinFromCategory(1, 2, "otherUser", false);

        Assert.False(result);
    }

    [Fact]
    public void RemovePinFromCategory_UserIsOwner_RemovesAndReturnsTrue()
    {
        var pc = new PinCategory();
        var category = new Category { AppUserId = "user123" };

        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetPinCategory(1, 2)).Returns(pc);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(category);

        var service = new CategoryService(repoMock.Object);

        var result = service.RemovePinFromCategory(1, 2, "user123", false);

        Assert.True(result);
        repoMock.Verify(r => r.RemovePinCategory(pc), Times.Once);
    }

    [Fact]
    public void RemovePinFromCategory_UserIsAdmin_RemovesAndReturnsTrue()
    {
        var pc = new PinCategory();
        var category = new Category { AppUserId = "owner" };

        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetPinCategory(1, 2)).Returns(pc);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(category);

        var service = new CategoryService(repoMock.Object);

        var result = service.RemovePinFromCategory(1, 2, "adminUser", true);

        Assert.True(result);
        repoMock.Verify(r => r.RemovePinCategory(pc), Times.Once);
    }
    
    
    // DeleteCategory
    // 1. categoria nu exista -> clasa de echivalenta
    // 2. user ul nu e owner si nici admin -> clasa de echivalenta
    // 3. user ul este owner -> clasa de echivalenta
    // 4. user ul nu e owner, dar e admin -> clasa de echivalenta
    
    [Fact]
    public void DeleteCategory_CategoryNotFound_ReturnsFalse()
    {
        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns((Category)null);

        var service = new CategoryService(repoMock.Object);

        var result = service.DeleteCategory(1, "user123", false);

        Assert.False(result);
    }
    
    [Fact]
    public void DeleteCategory_UserNotOwnerAndNotAdmin_ReturnsFalse()
    {
        var category = new Category { AppUserId = "owner" };

        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(category);

        var service = new CategoryService(repoMock.Object);

        var result = service.DeleteCategory(1, "otherUser", false);

        Assert.False(result);
    }
    
    [Fact]
    public void DeleteCategory_UserIsOwner_RemovesAndReturnsTrue()
    {
        var category = new Category { AppUserId = "user123" };

        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(category);

        var service = new CategoryService(repoMock.Object);

        var result = service.DeleteCategory(1, "user123", false);

        Assert.True(result);
        repoMock.Verify(r => r.RemoveCategory(category), Times.Once);
    }

    [Fact]
    public void DeleteCategory_UserIsAdmin_RemovesAndReturnsTrue()
    {
        var category = new Category { AppUserId = "owner" };

        var repoMock = new Mock<CategoryRepository>(null);
        repoMock.Setup(r => r.GetCategoryById(1)).Returns(category);

        var service = new CategoryService(repoMock.Object);

        var result = service.DeleteCategory(1, "adminUser", true);

        Assert.True(result);
        repoMock.Verify(r => r.RemoveCategory(category), Times.Once);
    }
}