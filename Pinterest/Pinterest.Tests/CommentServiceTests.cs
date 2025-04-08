using Moq;
using Pinterest.Models;
using Pinterest.Repositories;
using Pinterest.Services;

namespace Pinterest.Tests
{
    public class CommentServiceTests
    {
        // GetCommentById
        
        // commentId (int)
        // "1" (valid) - t1
        // "999" nu exista (invalid) - t2
        // null (invalid) - t3
        
        [Fact]
        public void GetCommentById_ValidId_ReturnsComment_t1()
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
        public void GetCommentById_InvalidId_ReturnsNull_t2()
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
        public void GetCommentById_NullId_ReturnsNull_t3()
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
        
        // comment_text
        // "text" (valid) - t1
        // "" (invalid) - t2
        // null (invalid) - t3
        
        // pinId
        // exista in db (valid) - t1
        // nu exista in db (invalid) - t4
        
        // userId
        // "string" (valid) - t1
        // "" (invalid) - t5
        // null (invalid) - t6
        
        [Fact]
        public void CreateComment_ValidInputs_t1()
        {
            var comment = new Comment { Text = "Hello world" };
            var repoMock = new Mock<CommentRepository>(null);
            var pinRepoMock = new Mock<PinRepository>(null);
            pinRepoMock.Setup(p => p.GetPinById(1)).Returns(new Pin { Id = 1 });

            var service = new CommentService(repoMock.Object, pinRepoMock.Object);

            var result = service.CreateComment(comment, 1, "user123");

            Assert.True(result.success);
            Assert.Null(result.error);
            repoMock.Verify(r => r.Add(It.IsAny<Comment>()), Times.Once);
        }
        
        [Fact]
        public void CreateComment_EmptyText_t2()
        {
            var comment = new Comment { Text = "" };
            var repoMock = new Mock<CommentRepository>(null);
            var pinRepoMock = new Mock<PinRepository>(null);
            pinRepoMock.Setup(p => p.GetPinById(1)).Returns(new Pin { Id = 1 });

            var service = new CommentService(repoMock.Object, pinRepoMock.Object);

            var result = service.CreateComment(comment, 1, "user123");

            Assert.False(result.success);
            Assert.Equal("Text cannot be empty.", result.error);
            repoMock.Verify(r => r.Add(It.IsAny<Comment>()), Times.Never);
        }
        
        [Fact]
        public void CreateComment_NullText_t3()
        {
            var comment = new Comment { Text = null };
            var repoMock = new Mock<CommentRepository>(null);
            var pinRepoMock = new Mock<PinRepository>(null);
            pinRepoMock.Setup(p => p.GetPinById(1)).Returns(new Pin { Id = 1 });

            var service = new CommentService(repoMock.Object, pinRepoMock.Object);

            var result = service.CreateComment(comment, 1, "user123");

            Assert.False(result.success);
            Assert.Equal("Text cannot be empty.", result.error);
        }
        
        [Fact]
        public void CreateComment_InvalidPin_t4()
        {
            var comment = new Comment { Text = "Valid Text" };
            var repoMock = new Mock<CommentRepository>(null);
            var pinRepoMock = new Mock<PinRepository>(null);
            pinRepoMock.Setup(p => p.GetPinById(999)).Returns((Pin)null);

            var service = new CommentService(repoMock.Object, pinRepoMock.Object);

            var result = service.CreateComment(comment, 999, "user123");

            Assert.False(result.success);
            Assert.Equal("Invalid pin ID.", result.error);
        }
        
        [Fact]
        public void CreateComment_EmptyUserId_t5()
        {
            var comment = new Comment { Text = "Test" };
            var repoMock = new Mock<CommentRepository>(null);
            var pinRepoMock = new Mock<PinRepository>(null);
            pinRepoMock.Setup(p => p.GetPinById(1)).Returns(new Pin { Id = 1 });

            var service = new CommentService(repoMock.Object, pinRepoMock.Object);

            var result = service.CreateComment(comment, 1, "");

            Assert.False(result.success);
            Assert.Equal("User ID cannot be empty.", result.error);
        }

        [Fact]
        public void CreateComment_NullUserId_t6()
        {
            var comment = new Comment { Text = "Test" };
            var repoMock = new Mock<CommentRepository>(null);
            var pinRepoMock = new Mock<PinRepository>(null);
            pinRepoMock.Setup(p => p.GetPinById(1)).Returns(new Pin { Id = 1 });

            var service = new CommentService(repoMock.Object, pinRepoMock.Object);

            var result = service.CreateComment(comment, 1, null);

            Assert.False(result.success);
            Assert.Equal("User ID cannot be empty.", result.error);
        }

        
        // UpdateComment
        
        // commentId
        // exista in db (valid) - t1
        // nu exista in db (invalid) - t2
        
        // new_comment_text
        // "text" (valid) - t1
        // "" (invalid) - t3
        // null (invalid) - t4
        
        [Fact]
        protected void UpdateComment_ValidInputs_ReturnsSuccess_t1()
        {
            var existingComment = new Comment { Id = 1, Text = "Old" };
            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(1)).Returns(existingComment);

            var service = new CommentService(repoMock.Object);
            var newData = new Comment { Text = "Updated" };

            var result = service.UpdateComment(1, newData);

            Assert.True(result.success);
            Assert.Null(result.error);
            Assert.NotNull(result.comment);
            Assert.Equal("Updated", result.comment.Text);
            repoMock.Verify(r => r.Update(existingComment), Times.Once);
        }

        [Fact]
        public void UpdateComment_InvalidId_ReturnsCommentNotFound_t2()
        {
            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(999)).Returns((Comment)null);

            var service = new CommentService(repoMock.Object);
            var newData = new Comment { Text = "Updated" };

            var result = service.UpdateComment(999, newData);

            Assert.False(result.success);
            Assert.Equal("Comment not found.", result.error);
            Assert.Null(result.comment);
            repoMock.Verify(r => r.Update(It.IsAny<Comment>()), Times.Never);
        }
        
        [Fact]
        public void UpdateComment_EmptyText_ReturnsError_t3()
        {
            var existingComment = new Comment { Id = 1, Text = "Old" };
            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(1)).Returns(existingComment);

            var service = new CommentService(repoMock.Object);
            var newData = new Comment { Text = "" };

            var result = service.UpdateComment(1, newData);

            Assert.False(result.success);
            Assert.Equal("Text cannot be empty.", result.error);
            Assert.Null(result.comment);
            repoMock.Verify(r => r.Update(It.IsAny<Comment>()), Times.Never);
        }

        [Fact]
        public void UpdateComment_NullText_ReturnsError_t4()
        {
            var existingComment = new Comment { Id = 1, Text = "Old" };
            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(1)).Returns(existingComment);

            var service = new CommentService(repoMock.Object);
            var newData = new Comment { Text = null };

            var result = service.UpdateComment(1, newData);

            Assert.False(result.success);
            Assert.Equal("Text cannot be empty.", result.error);
            Assert.Null(result.comment);
            repoMock.Verify(r => r.Update(It.IsAny<Comment>()), Times.Never);
        }
        
        
        // DeleteComment
        
        // commendId
        // exista in db (valid) - t1
        // nu exista in db (invalid) - t2
        
        // currentUserId + isAdmin (true/false)
        // utilizatorul e owner (valid) - t1 
        // utilizatorul nu e owner, dar e admin (isAdmin = true) (valid) - t3
        // utilizatorul nu e owner, nici admin (isAdmin = false) (invalid) - t4
        // "" (invalid) - t5
        // null (invalid) - t6

        [Fact]
        public void TryDeleteComment_CommentExists_UserIsOwner_ReturnsTrue_t1()
        {
            var comment = new Comment { Id = 1, AppUserId = "user123", PinId = 42 };
            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(1)).Returns(comment);

            var service = new CommentService(repoMock.Object);

            var result = service.TryDeleteComment(1, "user123", false, out var pinId);

            Assert.True(result);
            Assert.Equal(42, pinId);
            repoMock.Verify(r => r.Delete(comment), Times.Once);
        }
        
        [Fact]
        public void TryDeleteComment_CommentDoesNotExist_ReturnsFalse_t2()
        {
            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(999)).Returns((Comment)null);

            var service = new CommentService(repoMock.Object);

            var result = service.TryDeleteComment(999, "user", false, out var pinId);

            Assert.False(result);
            Assert.Null(pinId);
            repoMock.Verify(r => r.Delete(It.IsAny<Comment>()), Times.Never);
        }
        
        [Fact]
        public void TryDeleteComment_CommentExists_UserIsAdmin_ReturnsTrue_t3()
        {
            var comment = new Comment { Id = 1, AppUserId = "ownerUser", PinId = 99 };
            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(1)).Returns(comment);

            var service = new CommentService(repoMock.Object);

            var result = service.TryDeleteComment(1, "adminUser", true, out var pinId);

            Assert.True(result);
            Assert.Equal(99, pinId);
            repoMock.Verify(r => r.Delete(comment), Times.Once);
        }
        
        [Fact]
        public void TryDeleteComment_CommentExists_UserNotOwnerNorAdmin_ReturnsFalse_t4()
        {
            var comment = new Comment { Id = 1, AppUserId = "ownerUser", PinId = 77 };
            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(1)).Returns(comment);

            var service = new CommentService(repoMock.Object);

            var result = service.TryDeleteComment(1, "randomUser", false, out var pinId);

            Assert.False(result);
            Assert.Equal(77, pinId);
            repoMock.Verify(r => r.Delete(It.IsAny<Comment>()), Times.Never);
        }
        
        [Fact]
        public void TryDeleteComment_EmptyUserId_ThrowsException_t5()
        {
            var comment = new Comment { Id = 1, AppUserId = "user123", PinId = 10 };
            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(1)).Returns(comment);

            var service = new CommentService(repoMock.Object);

            Assert.Throws<ArgumentException>(() => service.TryDeleteComment(1, "", false, out _));
        }

        [Fact]
        public void TryDeleteComment_NullUserId_ThrowsException_t6()
        {
            var comment = new Comment { Id = 1, AppUserId = "user123", PinId = 10 };
            var repoMock = new Mock<CommentRepository>(null);
            repoMock.Setup(r => r.GetById(1)).Returns(comment);

            var service = new CommentService(repoMock.Object);

            Assert.Throws<ArgumentException>(() => service.TryDeleteComment(1, null, false, out _));
        }

    }
}
