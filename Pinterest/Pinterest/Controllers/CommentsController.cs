using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pinterest.Models;
using Pinterest.Data;
using Pinterest.Services;
using Pinterest.Repositories;

namespace Pinterest.Controllers;

public class CommentsController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly CommentService _commentService;

    public CommentsController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _env = env;
        _userManager = userManager;
        _roleManager = roleManager;

        var commentRepo = new CommentRepository(context);
        var pinRepo = new PinRepository(context);
        _commentService = new CommentService(commentRepo, pinRepo);
    }

    [HttpGet]
    public IActionResult Show(int? id)
    {
        var comment = _commentService.GetCommentById(id);
        ViewBag.Comment = comment;

        return View();
    }

    [Authorize(Roles = "User")]
    [HttpGet]
    public IActionResult New(int? pinId)
    {
        ViewBag.PinId = pinId;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> New(Comment newComment, int pinId)
    {
        var userId = _userManager.GetUserId(User);
        var (success, _) = _commentService.CreateComment(newComment, pinId, userId);

        if (success)
        {
            return RedirectToAction("Show", "Pins", new { id = newComment.PinId });
        }

        ViewBag.PinId = pinId;
        return View(newComment);
    }

    [Authorize(Roles = "User")]
    [HttpGet]
    public IActionResult Edit(int? id)
    {
        var comment = _commentService.GetCommentById(id);
        var currentUserId = _userManager.GetUserId(User);

        if (comment?.AppUserId != currentUserId)
        {
            return Forbid();
        }

        return View(comment);
    }

    [Authorize(Roles = "User")]
    [HttpPost]
    public IActionResult Edit(int id, Comment requestComment)
    {
        var result = _commentService.UpdateComment(id, requestComment);

        if (result.success)
        {
            return RedirectToAction("Show", "Pins", new { id = result.comment?.PinId });
        }

        ViewBag.Comment = result.comment;
        return View(requestComment);
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var success = _commentService.TryDeleteComment(id, currentUser.Id, User.IsInRole("Admin"), out int? pinId);

        if (!success)
        {
            return Forbid();
        }

        return RedirectToAction("Show", "Pins", new { id = pinId });
    }
}