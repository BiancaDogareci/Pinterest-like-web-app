using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Pinterest.Models;
using Pinterest.Repositories;
using Pinterest.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinterest.Tests
{
    public class PinServiceTests
    {

        //    GetPins
        // search
        // "", "test", null (valid) - t1, t2, t3
        // nu exista invalid

        // page
        // <0 (invalid) - t9
        // 0 (invalid) - t8
        // 1 (valid) - t4
        // n (valid) - t5
        // n+1 (invalid) - t6
        // >n+1 (invalid) - t7

        // perPage
        // <0 (invalid) - t11
        // 0 (invalid) - t10
        // 1 (valid) - t12
        // >1 (valid) - t13

        [Fact]
        public void GetPins_NoSearch_t1() //Returns Paginated Pins Ordered By Likes
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
        public void GetPins_WithSearch_t2()
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
        public void GetPins_SearchNull_t3()
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
            var result = service.GetPins(search: null, page: 1, perPage: 2);

            // verificari
            Assert.Equal(2, result.Pins.Count());
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/Index/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetPins_PageOne_t4()
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
        public void GetPins_PageN_t5()
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
            var result = service.GetPins(search: "", page: 2, perPage: 2); // exista doar 2 pagini si se cere pagina 2

            // verificari
            Assert.Equal(1, result.Pins.Count()); // ultimul pin
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/Index/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetPins_PageNPlusOne_t6() // returns empty list
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
            var result = service.GetPins(search: "", page: 3, perPage: 2); // exista doar 2 pagini si se cere pagina 3 (n+1)

            // verificari
            Assert.Empty(result.Pins);
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/Index/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetPins_PageGreaterThanNPlusOne_t7() // returns empty list
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
            var result = service.GetPins(search: "", page: 5, perPage: 2); // exista doar 2 pagini si se cere pagina 5 (>n+1)

            // verificari
            Assert.Empty(result.Pins);
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/Index/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetPins_PageZero_t8()
        {
            var repoMock = new Mock<PinRepository>(null);

            var allPins = new List<Pin>
            {
                new Pin { Id = 1, Title = "A", LikesCount = 10 },
                new Pin { Id = 2, Title = "B", LikesCount = 5 },
                new Pin { Id = 3, Title = "C", LikesCount = 3 }
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByLikes()).Returns(allPins);

            var service = new PinService(repoMock.Object);

            var result = service.GetPins(search: "", page: 0, perPage: 2);

            Assert.Empty(result.Pins);
            Assert.Equal(0, result.LastPage);
            Assert.Equal("/Pins/Index/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetPins_PageNegative_t9()
        {
            var repoMock = new Mock<PinRepository>(null);

            var allPins = new List<Pin>
            {
                new Pin { Id = 1, Title = "A", LikesCount = 10 },
                new Pin { Id = 2, Title = "B", LikesCount = 5 },
                new Pin { Id = 3, Title = "C", LikesCount = 3 }
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByLikes()).Returns(allPins);

            var service = new PinService(repoMock.Object);

            var result = service.GetPins(search: "", page: -1, perPage: 2);

            Assert.Empty(result.Pins);
            Assert.Equal(0, result.LastPage);
            Assert.Equal("/Pins/Index/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetPins_PerPageZero_t10()
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

        [Fact]
        public void GetPins_PerPageNegative_t11()
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
            var result = service.GetPins(search: "", page: 1, perPage: -1);

            // verificari
            Assert.Empty(result.Pins);
            Assert.Equal(0, result.LastPage);
            Assert.Equal("/Pins/Index/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetPins_PerPageOne_t12()
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
            var result = service.GetPins(search: "", page: 1, perPage: 1);

            // verificari
            Assert.Equal(1, result.Pins.Count()); // primul pin
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/Index/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetPins_PerPageGreaterThanOne_t13()
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
            var result = service.GetPins(search: "", page: 1, perPage: 2);

            // verificari
            Assert.Equal(2, result.Pins.Count());
            Assert.Equal(1, result.LastPage);
            Assert.Equal("/Pins/Index/?page", result.PaginationUrl);
        }



        //    GetRecentPins
        // search
        // "", "test", null (valid) - t1, t2, t3
        // nu exista invalid

        // page
        // <0 (invalid) - t9
        // 0 (invalid) - t8
        // 1 (valid) - t4
        // n (valid) - t5
        // n+1 (invalid) - t6
        // >n+1 (invalid) - t7

        // perPage
        // <0 (invalid) - t11
        // 0 (invalid) - t10
        // 1 (valid) - t12
        // >1 (valid) - t13

        [Fact]
        public void GetRecentPins_NoSearch_t1() //Returns Paginated Pins Ordered By Date
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var allPins = new List<Pin>
            {
                new Pin { Id = 1, Title = "A", Date = DateTime.Now.AddDays(-1) },
                new Pin { Id = 2, Title = "B", Date = DateTime.Now.AddDays(-2) },
                new Pin { Id = 3, Title = "C", Date = DateTime.Now.AddDays(-3) }
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
        public void GetRecentPins_WithSearch_t2()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var filteredPins = new List<Pin>
            {
                new Pin { Id = 1, Title = "testA", Date = DateTime.Now.AddDays(-1) },
                new Pin { Id = 2, Title = "testB", Date = DateTime.Now.AddDays(-2) }
            }.AsQueryable();

            repoMock.Setup(r => r.GetPinsBySearch("test")).Returns(filteredPins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetRecentPinsWithSearch(search: "test", page: 1, perPage: 1);

            // verificari
            Assert.Single(result.Pins);
            Assert.Equal(2, result.LastPage); // 2 obiecte in total, 1 pe pagina
            Assert.Equal("/Pins/IndexRecent/?search=test&page", result.PaginationUrl);
        }

        [Fact]
        public void GetRecentPins_SearchNull_t3()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var allPins = new List<Pin>
            {
                new Pin { Id = 1, Title = "A", Date = DateTime.Now.AddDays(-1) },
                new Pin { Id = 2, Title = "B", Date = DateTime.Now.AddDays(-2) },
                new Pin { Id = 3, Title = "C", Date = DateTime.Now.AddDays(-3) }
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByDate()).Returns(allPins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetRecentPinsWithSearch(search: null, page: 1, perPage: 2);

            // verificari
            Assert.Equal(2, result.Pins.Count());
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetRecentPins_PageOne_t4()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var pins = new List<Pin>
            {
                new Pin { Id = 1, Date = DateTime.Now.AddDays(-1) },
                new Pin { Id = 2, Date = DateTime.Now.AddDays(-2) },
                new Pin { Id = 3, Date = DateTime.Now.AddDays(-3) }
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByDate()).Returns(pins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetRecentPinsWithSearch(search: "", page: 1, perPage: 2);

            // verificari
            Assert.Equal(2, result.Pins.Count()); // primele 2
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetRecentPins_PageN_t5()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var pins = new List<Pin>
            {
                new Pin { Id = 1, Date = DateTime.Now.AddDays(-1) },
                new Pin { Id = 2, Date = DateTime.Now.AddDays(-2) },
                new Pin { Id = 3, Date = DateTime.Now.AddDays(-3) }
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByDate()).Returns(pins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetRecentPinsWithSearch(search: "", page: 2, perPage: 2); // exista doar 2 pagini si se cere pagina 2

            // verificari
            Assert.Equal(1, result.Pins.Count()); // ultimul pin
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetRecentPins_PageNPlusOne_t6() // returns empty list
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var pins = new List<Pin>
            {
                new Pin { Id = 1, Date = DateTime.Now.AddDays(-1) },
                new Pin { Id = 2, Date = DateTime.Now.AddDays(-2) },
                new Pin { Id = 3, Date = DateTime.Now.AddDays(-3) }
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByDate()).Returns(pins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetRecentPinsWithSearch(search: "", page: 3, perPage: 2); // exista doar 2 pagini si se cere pagina 3 (n+1)

            // verificari
            Assert.Empty(result.Pins);
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetRecentPins_PageGreaterThanNPlusOne_t7() // returns empty list
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var pins = new List<Pin>
            {
                new Pin { Id = 1, Date = DateTime.Now.AddDays(-1) },
                new Pin { Id = 2, Date = DateTime.Now.AddDays(-2) },
                new Pin { Id = 3, Date = DateTime.Now.AddDays(-3) }
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByDate()).Returns(pins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetRecentPinsWithSearch(search: "", page: 5, perPage: 2); // exista doar 2 pagini si se cere pagina 5 (>n+1)

            // verificari
            Assert.Empty(result.Pins);
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetRecentPins_PageZero_t8()
        {
            var repoMock = new Mock<PinRepository>(null);

            var allPins = new List<Pin>
            {
                new Pin { Id = 1, Title = "A", Date = DateTime.Now.AddDays(-1) },
                new Pin { Id = 2, Title = "B", Date = DateTime.Now.AddDays(-2) },
                new Pin { Id = 3, Title = "C", Date = DateTime.Now.AddDays(-3) }
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByDate()).Returns(allPins);

            var service = new PinService(repoMock.Object);

            var result = service.GetRecentPinsWithSearch(search: "", page: 0, perPage: 2);

            Assert.Empty(result.Pins);
            Assert.Equal(0, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetRecentPins_PageNegative_t9()
        {
            var repoMock = new Mock<PinRepository>(null);

            var allPins = new List<Pin>
            {
                new Pin { Id = 1, Title = "A", Date = DateTime.Now.AddDays(-1) },
                new Pin { Id = 2, Title = "B", Date = DateTime.Now.AddDays(-2) },
                new Pin { Id = 3, Title = "C", Date = DateTime.Now.AddDays(-3) }
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByDate()).Returns(allPins);

            var service = new PinService(repoMock.Object);

            var result = service.GetRecentPinsWithSearch(search: "", page: -1, perPage: 2);

            Assert.Empty(result.Pins);
            Assert.Equal(0, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetRecentPins_PerPageZero_t10()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var pins = new List<Pin>
            {
                new Pin { Id = 1, Date = DateTime.Now.AddDays(-1) },
                new Pin { Id = 2, Date = DateTime.Now.AddDays(-2) },
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByDate()).Returns(pins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetRecentPinsWithSearch(search: "", page: 1, perPage: 0);

            // verificari
            Assert.Empty(result.Pins);
            Assert.Equal(0, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetRecentPins_PerPageNegative_t11()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var pins = new List<Pin>
            {
                new Pin { Id = 1, Date = DateTime.Now.AddDays(-1) },
                new Pin { Id = 2, Date = DateTime.Now.AddDays(-2) },
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByDate()).Returns(pins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetRecentPinsWithSearch(search: "", page: 1, perPage: -1);

            // verificari
            Assert.Empty(result.Pins);
            Assert.Equal(0, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetRecentPins_PerPageOne_t12()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var pins = new List<Pin>
            {
                new Pin { Id = 1, Date = DateTime.Now.AddDays(-1) },
                new Pin { Id = 2, Date = DateTime.Now.AddDays(-2) },
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByDate()).Returns(pins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetRecentPinsWithSearch(search: "", page: 1, perPage: 1);

            // verificari
            Assert.Equal(1, result.Pins.Count()); // primul pin
            Assert.Equal(2, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?page", result.PaginationUrl);
        }

        [Fact]
        public void GetRecentPins_PerPageGreaterThanOne_t13()
        {
            // date si mock pentru PinRepository
            var repoMock = new Mock<PinRepository>(null);

            var pins = new List<Pin>
            {
                new Pin { Id = 1, Date = DateTime.Now.AddDays(-1)},
                new Pin { Id = 2, Date = DateTime.Now.AddDays(-2) },
            }.AsQueryable();

            repoMock.Setup(r => r.GetAllPinsOrderedByDate()).Returns(pins);

            var service = new PinService(repoMock.Object);

            // apelam functia
            var result = service.GetRecentPinsWithSearch(search: "", page: 1, perPage: 2);

            // verificari
            Assert.Equal(2, result.Pins.Count());
            Assert.Equal(1, result.LastPage);
            Assert.Equal("/Pins/IndexRecent/?page", result.PaginationUrl);
        }



        //    GetPinDetails
        // pin - existent sau inexistent

        // pin inexistent - t1
        // pin existent + fara like + fara categorii - t2
        // pin existent + fara like + cu categorii - t3
        // pin existent + cu like + fara categorii - t4
        // pin existent + cu like + cu categorii - t5

        [Fact]
        public void GetPinDetails_PinNotFound_t1()
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
        public void GetPinDetails_PinExists_NotLiked_NoCategories_t2()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user2" };
            var pin = new Pin { Id = 2 };

            var categories = new List<Category>();

            repoMock.Setup(r => r.GetPinWithCommentsAndAuthors(2)).Returns(pin);
            repoMock.Setup(r => r.GetAvailableCategoriesForUser("user2", 2)).Returns(categories);
            repoMock.Setup(r => r.HasUserLikedPin(2, "user2")).Returns(false);

            var result = service.GetPinDetails(2, user);

            Assert.Equal(pin, result.Pin);
            Assert.Empty(result.Categories);
            Assert.False(result.HasLiked);
        }

        [Fact]
        public void GetPinDetails_PinExists_NotLiked_CategoriesExist_t3()
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
        public void GetPinDetails_PinExists_Liked_NoCategories_t4()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user2" };
            var pin = new Pin { Id = 2 };

            var categories = new List<Category>();

            repoMock.Setup(r => r.GetPinWithCommentsAndAuthors(2)).Returns(pin);
            repoMock.Setup(r => r.GetAvailableCategoriesForUser("user2", 2)).Returns(categories);
            repoMock.Setup(r => r.HasUserLikedPin(2, "user2")).Returns(true);

            var result = service.GetPinDetails(2, user);

            Assert.Equal(pin, result.Pin);
            Assert.Empty(result.Categories);
            Assert.True(result.HasLiked);
        }

        [Fact]
        public void GetPinDetails_PinExists_Liked_CategoriesExist_t5()
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



        //    GetPinForGeneralView
        // pin gasit - t1
        // pin inexistent - t2
        [Fact]
        public void GetPinForGeneralView_PinExists_ReturnsPin_t1()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var pin = new Pin { Id = 1, Title = "A" };

            repoMock.Setup(r => r.GetPinWithCommentsAndAuthors(1)).Returns(pin);

            var result = service.GetPinForGeneralView(1);

            Assert.Equal(pin, result);
        }

        [Fact]
        public void GetPinForGeneralView_PinNotFound_ReturnsNull_t2()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            repoMock.Setup(r => r.GetPinWithCommentsAndAuthors(999)).Returns((Pin)null);

            var result = service.GetPinForGeneralView(999);

            Assert.Null(result);
        }



        //    CreatePinAsync
        // pt ca un pin sa fie valid:
        // Title - [Required], [StringLength(20)]
        // Description - [Required]
        // EmbeddedContentPath - [Required] (doar daca ai fisier)

        // MODEL
        // title
        // null (invalid) - t1
        // "" (invalid) - t2
        // 1 caracter (valid) - t3
        // 20 caractere (valid) - t4
        // 21 caractere (invalid) - t5
        // > 21 caractere (invalid) - t6

        // description
        // null (invalid) - t7
        // "" (invalid) - t8
        // > 0 caractere (valid) - t9

        // EmbeddedContentPath
        // null (invalid) - t10
        // "" (invalid) - t11
        // > 0 caractere (valid) - t12

        // FISIER
        // fisier imagine valid - t13
        // fisier cu tip invalid - t14

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
        public async Task CreatePinAsync_TitleNull_t1()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var pin = new Pin 
            { 
                Title = null, 
                Description = "Desc", 
                EmbeddedContentPath = "/x.jpg" 
            };

            var result = await service.CreatePinAsync(pin, null, user, env);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task CreatePinAsync_TitleEmpty_t2()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var pin = new Pin
            {
                Title = "",
                Description = "Desc",
                EmbeddedContentPath = "/x.jpg"
            };

            var result = await service.CreatePinAsync(pin, null, user, env);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task CreatePinAsync_TitleOneCharacter_t3()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var pin = new Pin
            {
                Title = "A",
                Description = "Desc",
                EmbeddedContentPath = "/x.jpg"
            };

            var result = await service.CreatePinAsync(pin, null, user, env);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task CreatePinAsync_Title20Characters_t4()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var pin = new Pin
            {
                Title = "AAAAAAAAAAAAAAAAAAAA",
                Description = "Desc",
                EmbeddedContentPath = "/x.jpg"
            };

            var result = await service.CreatePinAsync(pin, null, user, env);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task CreatePinAsync_Title21Characters_t5()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var pin = new Pin
            {
                Title = "AAAAAAAAAAAAAAAAAAAAA",
                Description = "Desc",
                EmbeddedContentPath = "/x.jpg"
            };

            var result = await service.CreatePinAsync(pin, null, user, env);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task CreatePinAsync_TitleGreaterThan21Characters_t6()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var pin = new Pin
            {
                Title = "AAAAAAAAAAAAAAAAAAAAAAAA",
                Description = "Desc",
                EmbeddedContentPath = "/x.jpg"
            };

            var result = await service.CreatePinAsync(pin, null, user, env);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task CreatePinAsync_DescriptionNull_t7()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var pin = new Pin
            {
                Title = "A",
                Description = null,
                EmbeddedContentPath = "/x.jpg"
            };

            var result = await service.CreatePinAsync(pin, null, user, env);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task CreatePinAsync_DescriptionEmpty_t8()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var pin = new Pin
            {
                Title = "A",
                Description = "",
                EmbeddedContentPath = "/x.jpg"
            };

            var result = await service.CreatePinAsync(pin, null, user, env);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task CreatePinAsync_DescriptionOneCharacter_t9()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var pin = new Pin
            {
                Title = "A",
                Description = "Desc",
                EmbeddedContentPath = "/x.jpg"
            };

            var result = await service.CreatePinAsync(pin, null, user, env);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task CreatePinAsync_EmbeddedContentPathNull_t10()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var pin = new Pin
            {
                Title = "A",
                Description = "Desc",
                EmbeddedContentPath = null
            };

            var result = await service.CreatePinAsync(pin, null, user, env);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task CreatePinAsync_EmbeddedContentPathEmpty_t11()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var pin = new Pin
            {
                Title = "A",
                Description = "Desc",
                EmbeddedContentPath = ""
            };

            var result = await service.CreatePinAsync(pin, null, user, env);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task CreatePinAsync_EmbeddedContentPathOneCharacter_t12()
        {
            var repoMock = new Mock<PinRepository>(null);
            var service = new PinService(repoMock.Object);

            var user = new AppUser { Id = "user1" };
            var env = Mock.Of<IWebHostEnvironment>(e => e.WebRootPath == "wwwroot");

            var pin = new Pin
            {
                Title = "A",
                Description = "Desc",
                EmbeddedContentPath = "a"
            };

            var result = await service.CreatePinAsync(pin, null, user, env);
            Assert.True(result.IsValid);
        }


        [Fact]
        public async Task CreatePinAsync_ValidImageFile_t13()
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
        public async Task CreatePinAsync_InvalidFileType_ReturnsUnsupportedMedia_t14()
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



        //    GetEditablePin
        // pin inexistent - t1
        // pin existent + user-ul e owner - t2
        // pin existent + user-ul nu e owner - t3

        [Fact]
        public void GetEditablePin_PinNotFound_ReturnsNullAndFalse_t1()
        {
            var repoMock = new Mock<PinRepository>(null);
            repoMock.Setup(r => r.GetPinById(1)).Returns((Pin)null);

            var service = new PinService(repoMock.Object);

            var result = service.GetEditablePin(1, "user123");

            Assert.Null(result.Pin);
            Assert.False(result.IsOwner);
        }

        [Fact]
        public void GetEditablePin_UserIsOwner_ReturnsPinAndTrue_t2()
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
        public void GetEditablePin_UserIsNotOwner_ReturnsPinAndFalse_t3()
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
        // pin inexistent - t1
        // pin existent + user-ul e owner - t2
        // pin existent + user-ul nu e owner - t3

        [Fact]
        public void TryUpdatePin_PinNotFound_ReturnsFalse_t1()
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
        public void TryUpdatePin_UserIsOwne_UpdatesAndReturnsTrue_t2()
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
        public void TryUpdatePin_UserIsNotOwner_ReturnsFalse_t3()
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
        // pin inexistent - t1
        // pin existent + user-ul nu e owner si nici admin - t2
        // pin existent + user-ul e owner + fisierul exista - t3
        // pin existent + user-ul e owner + fisierul nu exista - t4
        // pin existent + user-ul e admin + fisierul exista - t5
        // pin existent + user-ul e admin + fisierul nu exista - t6

        [Fact]
        public void TryDeletePin_PinNotFound_ReturnsFalse_t1()
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
        public void TryDeletePin_UserIsNotOwnerAndNotAdmin_ReturnsFalse_t2()
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
        public void TryDeletePin_UserIsOwner_FileExists_t3()
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
        public void TryDeletePin_UserIsOwner_FileDoesNotExist_DeletesPinOnly_t4()
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

        [Fact]
        public void TryDeletePin_UserIsAdmin_FileExists_t5()
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

            var result = service.TryDeletePin(1, new AppUser(), true, env);

            Assert.True(result);
            Assert.False(File.Exists(fullPath)); // fisierul ar trebui sa fie sters
            repoMock.Verify(r => r.DeletePin(pin), Times.Once);
        }

        [Fact]
        public void TryDeletePin_UserIsAdmin_FileDoesNotExist_DeletesPinOnly_t6()
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

            var result = service.TryDeletePin(1, new AppUser(), true, env);

            Assert.True(result);
            repoMock.Verify(r => r.DeletePin(pin), Times.Once);
        }



        //    ToggleLike
        // pin inexistent - t1
        // pin existent + utilizatorul nu a dat like - t2
        // pin existent + utilizatorul a dat like - t3

        [Fact]
        public void ToggleLike_PinDoesNotExist_t1()
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

        [Fact]
        public void ToggleLike_UserHasNotLikedBefore_ShouldAddLikeAndIncrementCount_t2()
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
        public void ToggleLike_UserAlreadyLiked_ShouldRemoveLikeAndDecrementCount_t3()
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


    }
}
