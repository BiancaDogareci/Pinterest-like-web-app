using Pinterest.Models;
using Pinterest.Repositories;

namespace Pinterest.Services;

public class CommentService
{
    private readonly CommentRepository _repo;
    private readonly PinRepository _pinRepo;
    
    public CommentService(CommentRepository repo, PinRepository pinRepo)
    {
        _repo = repo;
        _pinRepo = pinRepo;
    }
    
    public CommentService(CommentRepository repo)
    {
        _repo = repo;
        _pinRepo = null!;
    }

    public Comment? GetCommentById(int? id)
    {
        return _repo.GetById(id);
    }

    public (bool success, string? error) CreateComment(Comment comment, int pinId, string userId)
    {
        if (string.IsNullOrWhiteSpace(comment.Text))
            return (false, "Text cannot be empty.");

        if (_pinRepo == null)
            throw new InvalidOperationException("PinRepository is required for CreateComment.");

        var pinExists = _pinRepo.GetPinById(pinId) != null;
        if (!pinExists)
            return (false, "Invalid pin ID.");

        if (string.IsNullOrWhiteSpace(userId))
            return (false, "User ID cannot be empty.");

        comment.PinId = pinId;
        comment.AppUserId = userId;
        comment.Date = DateTime.Now;

        _repo.Add(comment);
        return (true, null);
    }

    public (bool success, string? error, Comment? comment) UpdateComment(int commentId, Comment newData)
    {
        var comment = _repo.GetById(commentId);
        if (comment == null)
            return (false, "Comment not found.", null);

        if (string.IsNullOrWhiteSpace(newData.Text))
            return (false, "Text cannot be empty.", null);

        comment.Text = newData.Text;
        comment.Date = DateTime.Now;
        _repo.Update(comment);

        return (true, null, comment);
    }

    public bool TryDeleteComment(int commentId, string currentUserId, bool isAdmin, out int? pinId)
    {
        if (string.IsNullOrWhiteSpace(currentUserId))
            throw new ArgumentException("User ID cannot be null or empty.", nameof(currentUserId));
        
        var comment = _repo.GetById(commentId);
        pinId = comment?.PinId;

        if (comment == null)
            return false;

        if (comment.AppUserId != currentUserId && !isAdmin)
            return false;

        _repo.Delete(comment);
        return true;
    }
}