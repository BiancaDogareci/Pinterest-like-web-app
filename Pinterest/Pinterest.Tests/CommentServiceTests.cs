using Moq;
using Pinterest.Models;
using Pinterest.Repositories;
using Pinterest.Services;

namespace Pinterest.Tests
{
    public class CommentServiceTests
    {
        // GetCommentById
        // 1. id-ul este valid si exista in baza de date -> clasa de echivalenta
        // 2. id-ul este valid, dar comentariul nu exista in baza de date -> clasa de echivalenta
        // 3. id-u este null -> analiza de frontiera
        
        [Fact]
        public void GetCommentById_ValidId_ReturnsComment()
        {
            // Arrange
            var repoMock = new Mock<CommentRepository>(null);
            var expectedComment = new Comment { Id = 1, Text = "Test" };

            repoMock.Setup(r => r.GetById(1)).Returns(expectedComment);

            var service = new CommentService(repoMock.Object);

            // Act
            var result = service.GetCommentById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test", result.Text);
        }

        [Fact]
        public void GetCommentById_InvalidId_ReturnsNull()
        {
            // Arrange
            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(999)).Returns((Comment)null);

            var service = new CommentService(repoMock.Object);

            // Act
            var result = service.GetCommentById(999);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public void GetCommentById_NullId_ReturnsNull()
        {
            // Arrange
            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(null)).Returns((Comment)null);

            var service = new CommentService(repoMock.Object);

            // Act
            var result = service.GetCommentById(null);

            // Assert
            Assert.Null(result);
        }

        
        // CreateComment
        // 1. comentariul este valid (nu e "" si nici null) -> clasa de echivalenta
        // 2. textul este gol "" -> analiza de frontiera
        // 3. textul este null -> analiza de frontiera

        [Fact]
        public void CreateComment_ValidComment_ReturnsSuccess()
        {
            // Arrange
            var repoMock = new Mock<CommentRepository>(null);
            var comment = new Comment { Text = "Nice!" };
            var service = new CommentService(repoMock.Object);

            // Act
            var result = service.CreateComment(comment, pinId: 1, userId: "user123");

            // Assert
            Assert.True(result.success);
            Assert.Null(result.error);
            repoMock.Verify(r => r.Add(It.IsAny<Comment>()), Times.Once);
        }

        [Fact]
        public void CreateComment_EmptyText_ReturnsError()
        {
            // Arrange
            var repoMock = new Mock<CommentRepository>(null);
            var comment = new Comment { Text = "" };
            var service = new CommentService(repoMock.Object);

            // Act
            var result = service.CreateComment(comment, pinId: 1, userId: "user123");

            // Assert
            Assert.False(result.success);
            Assert.Equal("Text cannot be empty.", result.error);
            repoMock.Verify(r => r.Add(It.IsAny<Comment>()), Times.Never);
        }
        
        [Fact]
        public void CreateComment_NullText_ReturnsError()
        {
            // Arrange
            var repoMock = new Mock<CommentRepository>(null);
            var comment = new Comment { Text = null };
            var service = new CommentService(repoMock.Object);

            // Act
            var result = service.CreateComment(comment, pinId: 1, userId: "user123");

            // Assert
            Assert.False(result.success);
            Assert.Equal("Text cannot be empty.", result.error);
            repoMock.Verify(r => r.Add(It.IsAny<Comment>()), Times.Never);
        }

        
        // UpdateComment
        // 1. comentariul exista, update valid -> clasa de echivalenta
        // 2. comentariul nu exista in baza de date, update invalid -> clasa de echivalenta
        // 3. comentariul se actualizeaza cu textul corect -> precizie in actualizare

        [Fact]
        public void UpdateComment_ValidUpdate_ReturnsUpdatedComment()
        {
            // Arrange
            var oldComment = new Comment { Id = 1, Text = "Old", Date = DateTime.MinValue };
            
            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(1)).Returns(oldComment);
            
            var service = new CommentService(repoMock.Object);
            var newData = new Comment { Text = "Updated!" };

            // Act
            var result = service.UpdateComment(1, newData);

            // Assert
            Assert.True(result.success);
            Assert.Equal("Updated!", result.comment.Text);
            Assert.NotEqual(DateTime.MinValue, result.comment.Date);
            repoMock.Verify(r => r.Update(oldComment), Times.Once);
        }
        
        [Fact]
        public void UpdateComment_CommentNotFound_ReturnsFalse()
        {
            // Arrange
            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(1)).Returns((Comment)null);

            var service = new CommentService(repoMock.Object);

            // Act
            var result = service.UpdateComment(1, new Comment { Text = "new" });

            // Assert
            Assert.False(result.success);
            Assert.Null(result.comment);
            repoMock.Verify(r => r.Update(It.IsAny<Comment>()), Times.Never);
        }
        
        [Fact]
        public void UpdateComment_UpdateTextIsCorrect()
        {
            // Arrange
            var existing = new Comment { Id = 1, Text = "Initial" };

            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(1)).Returns(existing);

            var service = new CommentService(repoMock.Object);
            var updated = new Comment { Text = "Updated!" };

            // Act
            var result = service.UpdateComment(1, updated);

            // Assert
            Assert.Equal("Updated!", result.comment.Text);
            Assert.True(result.success);
        }
        
        
        // DeleteComment
        // 1. utilizatorul e owner, delete valid -> clasa de echivalenta
        // 2. utilizatorul nu e owner, dar e admin, delete valid -> clasa de echivalenta
        // 3. comentariul nu exista, delete invalid -> clasa de echivalenta
        // 4. utilizator neautorizat, delete invalid -> clasa de echivalenta

        [Fact]
        public void TryDeleteComment_UserIsOwner_ReturnsTrue()
        {
            // Arrange
            var comment = new Comment { Id = 1, AppUserId = "user123", PinId = 42 };

            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(1)).Returns(comment);

            var service = new CommentService(repoMock.Object);

            // Act
            var result = service.TryDeleteComment(1, "user123", false, out var pinId);

            // Assert
            Assert.True(result);
            Assert.Equal(42, pinId);
            repoMock.Verify(r => r.Delete(comment), Times.Once);
        }
        
        [Fact]
        public void TryDeleteComment_UserIsAdmin_ReturnsTrue()
        {
            // Arrange
            var comment = new Comment { Id = 1, AppUserId = "user123", PinId = 99 };

            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(1)).Returns(comment);

            var service = new CommentService(repoMock.Object);

            // Act
            var result = service.TryDeleteComment(1, "otherUser", true, out var pinId);

            // Assert
            Assert.True(result);
            Assert.Equal(99, pinId);
            repoMock.Verify(r => r.Delete(comment), Times.Once);
        }
        
        [Fact]
        public void TryDeleteComment_CommentNotFound_ReturnsFalse()
        {
            // Arrange
            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(1)).Returns((Comment)null);

            var service = new CommentService(repoMock.Object);

            // Act
            var result = service.TryDeleteComment(1, "user1", false, out var pinId);

            // Assert
            Assert.False(result);
            Assert.Null(pinId);
            repoMock.Verify(r => r.Delete(It.IsAny<Comment>()), Times.Never);
        }
        
        [Fact]
        public void TryDeleteComment_UserNotOwnerNorAdmin_ReturnsFalse()
        {
            // Arrange
            var comment = new Comment { Id = 1, AppUserId = "otherUser", PinId = 7 };

            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(1)).Returns(comment);

            var service = new CommentService(repoMock.Object);

            // Act
            var result = service.TryDeleteComment(1, "currentUser", false, out var pinId);

            // Assert
            Assert.False(result);
            Assert.Equal(7, pinId);
            repoMock.Verify(r => r.Delete(It.IsAny<Comment>()), Times.Never);
        }
    }
}
