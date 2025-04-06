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

    public Comment? GetById(int? id)
    {
        return _db.Comments.Find(id);
    }

    public void Add(Comment comment)
    {
        _db.Comments.Add(comment);
        _db.SaveChanges();
    }

    public void Update(Comment comment)
    {
        _db.SaveChanges();
    }

    public void Delete(Comment comment)
    {
        _db.Comments.Remove(comment);
        _db.SaveChanges();
    }

    public AppUser? GetUserById(string userId)
    {
        return _db.AppUsers.Find(userId);
    }
}