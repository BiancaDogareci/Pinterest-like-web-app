using Pinterest.Data;
using Pinterest.Models;
using Microsoft.EntityFrameworkCore;

namespace Pinterest.Repositories;

public class CommentRepository
{
    private readonly ApplicationDbContext _db;

    public CommentRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public virtual Comment? GetById(int? id)
    {
        return _db.Comments.Find(id);
    }

    public virtual void Add(Comment comment)
    {
        _db.Comments.Add(comment);
        _db.SaveChanges();
    }

    public virtual void Update(Comment comment)
    {
        _db.SaveChanges();
    }

    public virtual void Delete(Comment comment)
    {
        _db.Comments.Remove(comment);
        _db.SaveChanges();
    }

    public virtual AppUser? GetUserById(string userId)
    {
        return _db.AppUsers.Find(userId);
    }
}