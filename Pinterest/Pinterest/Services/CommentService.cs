using Pinterest.Models;
using Pinterest.Repositories;

namespace Pinterest.Services;

public class CommentService
{
    private readonly CommentRepository _repo;

    public CommentService(CommentRepository repo)
    {
        _repo = repo;
    }

    public Comment? GetCommentById(int? id)
    {
        return _repo.GetById(id);
    }

    public (bool success, string? error) CreateComment(Comment comment, int pinId, string userId)
    {
        comment.PinId = pinId;
        comment.AppUserId = userId;
        comment.Date = DateTime.Now;

        if (string.IsNullOrWhiteSpace(comment.Text))
            return (false, "Text cannot be empty.");

        _repo.Add(comment);
        return (true, null);
    }

    public (bool success, Comment? comment) UpdateComment(int commentId, Comment newData)
    {
        var comment = _repo.GetById(commentId);
        if (comment == null)
            return (false, null);

        comment.Text = newData.Text;
        comment.Date = DateTime.Now;
        _repo.Update(comment);

        return (true, comment);
    }

    public bool TryDeleteComment(int commentId, string currentUserId, bool isAdmin, out int? pinId)
    {
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