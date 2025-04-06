using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Pinterest.Models;
using Pinterest.Repositories;
using Pinterest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinterest.Tests
{
    public class PinServiceTests
    {
        //    GetPins
        // fara search (search = "") - clasa de echivalenta
        // cu search - clasa de echivalenta
        // valoarea minima valida a paginii - analiza de frontiera
        // valoarea mai mare fata de cea maxima valida a paginii - analiza de frontiera
        // perPage = 0 - analiza defensiva


        [Fact]
        public void GetPins_NoSearch_ReturnsPaginatedPinsOrderedByLikes()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var allPins = new List<Pin>
            {
                new Pin { Id = 1, Title = "A", LikesCount = 10 },
                new Pin { Id = 2, Title = "B", LikesCount = 5 },
                new Pin { Id = 3, Title = "C", LikesCount = 3 }
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByLikes()).Returns(allPins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetPins(search: "", page: 1, perPage: 2);

            // verificari
            Assert.Equal(2, result.Pins.Count());
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/Index/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetPins_WithSearch_ReturnsFilteredPins()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var filteredPins = new List<Pin>
            {
                new Pin { Id = 1, Title = "testA", LikesCount = 10 },
                new Pin { Id = 2, Title = "testB", LikesCount = 5 }
            }.AsQueryable();

            repoMock.Setup(r => r.GetPinsBySearch("test")).Returns(filteredPins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetPins(search: "test", page: 1, perPage: 1);

            // verificari
            Assert.Single(result.Pins);
            Assert.Equal(2, result.LastPage); // 2 obiecte in total, 1 pe pagina
            Assert.Equal("/Pins/Index/?search=test&page", result.PaginationUrl);
        }

        [Fact]
        public void GetPins_PageOne_ReturnsFirstPage()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var pins = new List<Pin>
            {
                new Pin { Id = 1 },
                new Pin { Id = 2 },
                new Pin { Id = 3 }
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByLikes()).Returns(pins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetPins(search: "", page: 1, perPage: 2);

            // verificari
            Assert.Equal(2, result.Pins.Count()); // primele 2
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/Index/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetPins_PageGreaterThanTotalPages_ReturnsEmptyList()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var pins = new List<Pin>
            {
                new Pin { Id = 1 },
                new Pin { Id = 2 },
                new Pin { Id = 3 }
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByLikes()).Returns(pins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetPins(search: "", page: 5, perPage: 2); // exista doar 2 pagini

            // verificari
            Assert.Empty(result.Pins);
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/Index/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetPins_PerPageZero_ShouldHandleGracefully()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var pins = new List<Pin>
            {
                new Pin { Id = 1 },
                new Pin { Id = 2 },
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByLikes()).Returns(pins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetPins(search: "", page: 1, perPage: 0);

            // verificari
            Assert.Empty(result.Pins);
            Assert.Equal(0, result.LastPage);
            Assert.Equal("/Pins/Index/?page", result.PaginationUrl);
        }



        //    GetRecentPins
        // fara search (search = "") - clasa de echivalenta
        // cu search - clasa de echivalenta
        // valoarea minima valida a paginii - analiza de frontiera
        // valoarea mai mare fata de cea maxima valida a paginii - analiza de frontiera
        // perPage = 0 - analiza defensiva

        [Fact]
        public void GetRecentPins_NoSearch_ReturnsPaginatedPinsOrderedByDate()
        {
            // aranjeaza datele si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var allPins = new List<Pin>
            {
                new Pin { Id = 1, Date = DateTime.Now.AddDays(-1) },
                new Pin { Id = 2, Date = DateTime.Now.AddDays(-2) },
                new Pin { Id = 3, Date = DateTime.Now.AddDays(-3) },
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByDate()).Returns(allPins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetRecentPinsWithSearch(search: "", page: 1, perPage: 2);

            // verificari
            Assert.Equal(2, result.Pins.Count());
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetRecentPins_WithSearch_ReturnsFilteredPins()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var filteredPins = new List<Pin>
            {
                new Pin { Id = 1, Title = "TestA" },
                new Pin { Id = 2, Title = "B" }
            }.AsQueryable();

            repoMock.Setup(r => r.GetPinsBySearch("Test")).Returns(filteredPins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetRecentPinsWithSearch("Test", 1, 1);

            // verificari
            Assert.Single(result.Pins);
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?search=Test&page", result.PaginationUrl);
        }

        [Fact]
        public void GetRecentPins_PageOne_ReturnsFirstPage()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var pins = new List<Pin>
            {
                new Pin { Id = 1 },
                new Pin { Id = 2 },
                new Pin { Id = 3 }
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByDate()).Returns(pins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetRecentPinsWithSearch("", 1, 2);

            // verificari
            Assert.Equal(2, result.Pins.Count());
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetRecentPins_PageTooHigh_ReturnsEmptyList()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var pins = new List<Pin>
            {
                new Pin { Id = 1 },
                new Pin { Id = 2 },
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByDate()).Returns(pins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetRecentPinsWithSearch("", 5, 1);

            // verificari
            Assert.Empty(result.Pins);
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetRecentPins_PerPageZero_ShouldReturnEmptyAndZeroPages()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var pins = new List<Pin>
            {
                new Pin { Id = 1 },
                new Pin { Id = 2 }
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByDate()).Returns(pins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetRecentPinsWithSearch("", 1, 0);

            // verificari
            Assert.Empty(result.Pins);
            Assert.Equal(0, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?page", result.PaginationUrl);
        }



        //    GetPinDetails
        // pin inexistent
        // pin existent + fara like + cu categorii
        // pin existent + cu like + cu categorii
        // pin existent + fara categorii
        [Fact]
        public void GetPinDetails_PinNotFound_ReturnsNullEmptyFalse()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user123" };

            repoMock.Setup(r => r.GetPinWithCommentsAndAuthors(1)).Returns((Pin)null);

            var result = service.GetPinDetails(1, user);

            Assert.Null(result.Pin);
            Assert.Empty(result.Categories);
            Assert.False(result.HasLiked);
        }

        [Fact]
        public void GetPinDetails_PinExists_NotLiked_CategoriesExist_ReturnsCorrectData()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user2" };
            var pin = new Pin { Id = 2 };

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "A" }
            };

            repoMock.Setup(r => r.GetPinWithCommentsAndAuthors(2)).Returns(pin);
            repoMock.Setup(r => r.GetAvailableCategoriesForUser("user2", 2)).Returns(categories);
            repoMock.Setup(r => r.HasUserLikedPin(2, "user2")).Returns(false);

            var result = service.GetPinDetails(2, user);

            Assert.Equal(pin, result.Pin);
            Assert.Equal(categories, result.Categories);
            Assert.False(result.HasLiked);
        }

        [Fact]
        public void GetPinDetails_PinExists_Liked_CategoriesExist_ReturnsCorrectData()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user2" };
            var pin = new Pin { Id = 2 };

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "A" }
            };

            repoMock.Setup(r => r.GetPinWithCommentsAndAuthors(2)).Returns(pin);
            repoMock.Setup(r => r.GetAvailableCategoriesForUser("user2", 2)).Returns(categories);
            repoMock.Setup(r => r.HasUserLikedPin(2, "user2")).Returns(true);

            var result = service.GetPinDetails(2, user);

            Assert.Equal(pin, result.Pin);
            Assert.Equal(categories, result.Categories);
            Assert.True(result.HasLiked);
        }

        [Fact]
        public void GetPinDetails_PinExists_NoCategories_ReturnsEmptyCategoryList()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var pin = new Pin { Id = 1 };

            repoMock.Setup(r => r.GetPinWithCommentsAndAuthors(1)).Returns(pin);
            repoMock.Setup(r => r.GetAvailableCategoriesForUser("user1", 1)).Returns(new List<Category>());
            repoMock.Setup(r => r.HasUserLikedPin(1, "user1")).Returns(false);

            var result = service.GetPinDetails(1, user);

            Assert.Equal(pin, result.Pin);
            Assert.Empty(result.Categories);
            Assert.False(result.HasLiked);
        }



        //    GetPinForGeneralView
        // pin gasit
        // pin inexistent
        [Fact]
        public void GetPinForGeneralView_PinExists_ReturnsPin()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var pin = new Pin { Id = 1, Title = "A" };

            repoMock.Setup(r => r.GetPinWithCommentsAndAuthors(1)).Returns(pin);

            var result = service.GetPinForGeneralView(1);

            Assert.Equal(pin, result);
        }

        [Fact]
        public void GetPinForGeneralView_PinNotFound_ReturnsNull()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            repoMock.Setup(r => r.GetPinWithCommentsAndAuthors(999)).Returns((Pin)null);

            var result = service.GetPinForGeneralView(999);

            Assert.Null(result);
        }



        //    CreatePinAsync
        // fisier imagine valid + model valid
        // fisier cu tip invalid + model valid
        // model invalid
        // fara fisier
        private IFormFile CreateMockFile(string fileName, string contentType)
        {
            var content = "Fake content";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

            var mock = new Mock<IFormFile>();
            mock.Setup(f => f.Length).Returns(stream.Length);
            mock.Setup(f => f.FileName).Returns(fileName);
            mock.Setup(f => f.ContentType).Returns(contentType);
            mock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                .Returns<Stream, CancellationToken>((targetStream, _) => stream.CopyToAsync(targetStream));

            return mock.Object;
        }

        [Fact]
        public async Task CreatePinAsync_ValidImageFile_ValidModel_ReturnsTrue()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var file = CreateMockFile("image.jpg", "image/jpeg");

            var pin = new Pin
            {
                Title = "Title",
                Description = "Description"
            };

            var result = await service.CreatePinAsync(pin, file, user, env);

            Assert.True(result.IsValid);
            Assert.Null(result.ErrorMessage);
            repoMock.Verify(r => r.AddPin(pin), Times.Once);
        }

        [Fact]
        public async Task CreatePinAsync_InvalidFileType_ReturnsUnsupportedMedia()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var file = CreateMockFile("file.txt", "text/plain");
            
            var pin = new Pin
            {
                Title = "Title",
                Description = "Description"
            };

            var result = await service.CreatePinAsync(pin, file, user, env);

            Assert.False(result.IsValid);
            Assert.Equal("Unsupported media type", result.ErrorMessage);
            repoMock.Verify(r => r.AddPin(It.IsAny<Pin>()), Times.Never);
        }

        [Fact]
        public async Task CreatePinAsync_InvalidModel_ReturnsValidationError()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var file = CreateMockFile("image.jpg", "image/jpeg");

            var pin = new Pin
            {
                Title = "", // invalid, nu poate sa fie null
                Description = "Description"
            };

            var result = await service.CreatePinAsync(pin, file, user, env);

            Assert.False(result.IsValid);
            Assert.Equal("Model validation failed.", result.ErrorMessage);
            repoMock.Verify(r => r.AddPin(It.IsAny<Pin>()), Times.Never);
        }

        [Fact]
        public async Task CreatePinAsync_NoFileProvided_StillValidIfModelValid()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var pin = new Pin
            {
                Title = "Title",
                Description = "Description"
            };

            var result = await service.CreatePinAsync(pin, null, user, env);

            Assert.False(result.IsValid);
            Assert.Equal("Model validation failed.", result.ErrorMessage);
            repoMock.Verify(r => r.AddPin(It.IsAny<Pin>()), Times.Never);
        }



        //    GetEditablePin
        // pin inexistent
        // pin existent + user-ul e owner
        // pin existent + user-ul nu e owner
        [Fact]
        public void GetEditablePin_PinNotFound_ReturnsNullAndFalse()
        {
            var repoMock = new Mock<PinRepository>(null);
            repoMock.Setup(r => r.GetPinById(1)).Returns((Pin)null);

            var service = new PinService(repoMock.Object);

            var result = service.GetEditablePin(1, "user123");

            Assert.Null(result.Pin);
            Assert.False(result.IsOwner);
        }

        [Fact]
        public void GetEditablePin_UserIsOwner_ReturnsPinAndTrue()
        {
            var pin = new Pin 
            { 
                Id = 1, 
                AppUserId = "user123" 
            };

            var repoMock = new Mock<PinRepository>(null);
            repoMock.Setup(r => r.GetPinById(1)).Returns(pin);

            var service = new PinService(repoMock.Object);

            var result = service.GetEditablePin(1, "user123");

            Assert.Equal(pin, result.Pin);
            Assert.True(result.IsOwner);
        }

        [Fact]
        public void GetEditablePin_UserIsNotOwner_ReturnsPinAndFalse()
        {
            var pin = new Pin 
            { 
                Id = 1, 
                AppUserId = "user123" 
            };

            var repoMock = new Mock<PinRepository>(null);
            repoMock.Setup(r => r.GetPinById(1)).Returns(pin);

            var service = new PinService(repoMock.Object);

            var result = service.GetEditablePin(1, "user456");

            Assert.Equal(pin, result.Pin);
            Assert.False(result.IsOwner);
        }



        //    TryUpdatePin
        // pin inexistent
        // pin existent + user-ul e owner
        // pin existent + user-ul nu e owner
        [Fact]
        public void TryUpdatePin_PinNotFound_ReturnsFalse()
        {
            var repoMock = new Mock<PinRepository>(null);
            repoMock.Setup(r => r.GetPinById(1)).Returns((Pin)null);

            var service = new PinService(repoMock.Object);

            var updatedPin = new Pin 
            { 
                Title = "New", 
                Description = "Updated" 
            };

            var result = service.TryUpdatePin(1, updatedPin, "user123");

            Assert.False(result);
            repoMock.Verify(r => r.UpdatePin(It.IsAny<Pin>()), Times.Never);
        }

        [Fact]
        public void TryUpdatePin_UserIsOwne_UpdatesAndReturnsTrue()
        {
            var existingPin = new Pin
            {
                Id = 1,
                AppUserId = "user1",
                Title = "Old Title",
                Description = "Old Description"
            };

            var repoMock = new Mock<PinRepository>(null);
            repoMock.Setup(r => r.GetPinById(1)).Returns(existingPin);

            var service = new PinService(repoMock.Object);

            var updatedPin = new Pin
            {
                Title = "New Title",
                Description = "New Description"
            };

            var result = service.TryUpdatePin(1, updatedPin, "user1");

            Assert.True(result);
            Assert.Equal("New Title", existingPin.Title);
            Assert.Equal("New Description", existingPin.Description);
            repoMock.Verify(r => r.UpdatePin(existingPin), Times.Once);
        }

        [Fact]
        public void TryUpdatePin_UserIsNotOwner_ReturnsFalse()
        {
            var existingPin = new Pin 
            { 
                Id = 1, 
                AppUserId = "user1",
                Title = "Old Title",
                Description = "Old Description"
            };

            var repoMock = new Mock<PinRepository>(null);
            repoMock.Setup(r => r.GetPinById(1)).Returns(existingPin);

            var service = new PinService(repoMock.Object);

            var updatedPin = new Pin 
            {
                Title = "New Title",
                Description = "New Description"
            };

            var result = service.TryUpdatePin(1, updatedPin, "user111");

            Assert.False(result);
            repoMock.Verify(r => r.UpdatePin(It.IsAny<Pin>()), Times.Never);
        }



        //    TryDeletePin
        // pin inexistent
        // pin existent + user-ul nu e owner si nici admin
        // pin existent + user-ul e owner + fisierul exista
        // pin existent + user-ul e owner + fisierul nu exista
        [Fact]
        public void TryDeletePin_PinNotFound_ReturnsFalse()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser 
            { 
                Id = "user1" 
            };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "root");

            repoMock.Setup(r => r.GetPinById(1)).Returns((Pin)null);

            var result = service.TryDeletePin(1, user, false, env);

            Assert.False(result);
            repoMock.Verify(r => r.DeletePin(It.IsAny<Pin>()), Times.Never);
        }

        [Fact]
        public void TryDeletePin_UserIsNotOwnerAndNotAdmin_ReturnsFalse()
        {
            var pin = new Pin 
            { 
                Id = 1,
                AppUserId = "user123",
                EmbeddedContentPath = "/images/img.jpg"
            };

            var repoMock = new Mock<PinRepository>(null);
            repoMock.Setup(r => r.GetPinById(1)).Returns(pin);

            var service = new PinService(repoMock.Object);

            var currentUser = new AppUser 
            { 
                Id = "notOwner"
            };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "root");

            var result = service.TryDeletePin(1, currentUser, false, env);

            Assert.False(result);
            repoMock.Verify(r => r.DeletePin(It.IsAny<Pin>()), Times.Never);
        }

        [Fact]
        public void TryDeletePin_UserIsOwner_FileExists_DeletesFileAndPin()
        {
            var pin = new Pin
            {
                Id = 1,
                AppUserId = "user123",
                EmbeddedContentPath = "/images/img.jpg"
            };

            var repoMock = new Mock<PinRepository>(null);
            repoMock.Setup(r => r.GetPinById(1)).Returns(pin);

            var service = new PinService(repoMock.Object);

            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "testroot");
            var fullPath = Path.Combine(env.WebRootPath, pin.EmbeddedContentPath.TrimStart('/'));

            // fisier fals 
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            File.WriteAllText(fullPath, "mock data");

            var user = new AppUser 
            { 
                Id = "user123" 
            };

            var result = service.TryDeletePin(1, user, false, env);

            Assert.True(result);
            Assert.False(File.Exists(fullPath)); // fisierul ar trebui sa fie sters
            repoMock.Verify(r => r.DeletePin(pin), Times.Once);
        }

        [Fact]
        public void TryDeletePin_UserIsOwner_FileDoesNotExist_DeletesPinOnly()
        {
            var pin = new Pin
            {
                Id = 1,
                AppUserId = "user123",
                EmbeddedContentPath = "/images/missing.jpg"
            };

            var repoMock = new Mock<PinRepository>(null);
            repoMock.Setup(r => r.GetPinById(1)).Returns(pin);

            var service = new PinService(repoMock.Object);
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "testroot");

            var user = new AppUser 
            { 
                Id = "user123"
            };

            var result = service.TryDeletePin(1, user, false, env);

            Assert.True(result);
            repoMock.Verify(r => r.DeletePin(pin), Times.Once);
        }



        //    ToggleLike
        // pin existent + utilizatorul nu a dat like
        // pin existent + utilizatorul a dat like
        // pin inexistent

        [Fact]
        public void ToggleLike_UserHasNotLikedBefore_ShouldAddLikeAndIncrementCount()
        {
            var pinId = 1;
            var userId = "user123";

            var pin = new Pin
            {
                Id = pinId,
                LikesCount = 0
            };

            var repoMock = new Mock<PinRepository>(null); // null pentru ApplicationDbContext

            repoMock.Setup(r => r.GetPinById(pinId)).Returns(pin);
            repoMock.Setup(r => r.HasUserLikedPin(pinId, userId)).Returns(false);

            var service = new PinService(repoMock.Object);

            var result = service.ToggleLike(pinId, userId);

            Assert.True(result);
            Assert.Equal(1, pin.LikesCount);
            repoMock.Verify(r => r.AddLike(pinId, userId), Times.Once);
            repoMock.Verify(r => r.Save(), Times.Once);
        }

        [Fact]
        public void ToggleLike_UserAlreadyLiked_ShouldRemoveLikeAndDecrementCount()
        {
            var pinId = 1;
            var userId = "user123";

            var pin = new Pin
            {
                Id = pinId,
                LikesCount = 1
            };

            var repoMock = new Mock<PinRepository>(null);

            repoMock.Setup(r => r.GetPinById(pinId)).Returns(pin);
            repoMock.Setup(r => r.HasUserLikedPin(pinId, userId)).Returns(true);

            var service = new PinService(repoMock.Object);

            var result = service.ToggleLike(pinId, userId);

            Assert.True(result);
            Assert.Equal(0, pin.LikesCount);
            repoMock.Verify(r => r.RemoveLike(pinId, userId), Times.Once);
            repoMock.Verify(r => r.Save(), Times.Once);
        }

        [Fact]
        public void ToggleLike_PinDoesNotExist_ShouldReturnFalse()
        {
            var pinId = 1;
            var userId = "user123";

            var repoMock = new Mock<PinRepository>(null);
            repoMock.Setup(r => r.GetPinById(pinId)).Returns((Pin)null);

            var service = new PinService(repoMock.Object);

            var result = service.ToggleLike(pinId, userId);

            Assert.False(result);
            repoMock.Verify(r => r.HasUserLikedPin(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            repoMock.Verify(r => r.AddLike(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            repoMock.Verify(r => r.RemoveLike(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            repoMock.Verify(r => r.Save(), Times.Never);
        }


    }
}
