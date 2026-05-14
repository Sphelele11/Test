using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Test.Data;
using Test.Models;

[Authorize]
public class KnowledgeController : Controller
{
    private readonly ApplicationDbContext _db;
    public KnowledgeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? category, string? search)
    {
        var query = _db.KnowledgeArticles.Where(k => k.Status == ArticleStatus.Published);
        if (!string.IsNullOrEmpty(category) && Enum.TryParse<Category>(category, out var c))
            query = query.Where(k => k.Category == c);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(k => k.Title.Contains(search) || (k.Tags != null && k.Tags.Contains(search)) || (k.Summary != null && k.Summary.Contains(search)));
        return View(await query.OrderByDescending(k => k.Views).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var article = await _db.KnowledgeArticles.FindAsync(id);
        if (article == null) return NotFound();
        article.Views++;
        await _db.SaveChangesAsync();
        return View(article);
    }

    [Authorize(Roles = "Admin,Agent")]
    public IActionResult Create() => View(new KnowledgeArticle());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Create(KnowledgeArticle article)
    {
        if (!ModelState.IsValid) return View(article);
        article.ArticleNumber = $"KB-{(await _db.KnowledgeArticles.CountAsync() + 1):D3}";
        article.Author = User.Identity?.Name;
        article.CreatedAt = article.UpdatedAt = DateTime.UtcNow;
        if (article.Status == ArticleStatus.Published) article.PublishedAt = DateTime.UtcNow;
        _db.KnowledgeArticles.Add(article);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Article {article.ArticleNumber} created.";
        return RedirectToAction(nameof(Details), new { id = article.Id });
    }

    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Edit(int id)
    {
        var a = await _db.KnowledgeArticles.FindAsync(id);
        if (a == null) return NotFound();
        return View(a);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Edit(int id, KnowledgeArticle article)
    {
        if (!ModelState.IsValid) return View(article);
        var existing = await _db.KnowledgeArticles.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Title = article.Title; existing.Content = article.Content;
        existing.Summary = article.Summary; existing.Status = article.Status;
        existing.Category = article.Category; existing.Tags = article.Tags;
        existing.UpdatedAt = DateTime.UtcNow;
        if (article.Status == ArticleStatus.Published && existing.PublishedAt == null) existing.PublishedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Article updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Vote(int id, bool helpful)
    {
        var article = await _db.KnowledgeArticles.FindAsync(id);
        if (article != null)
        {
            if (helpful) article.HelpfulVotes++; else article.UnhelpfulVotes++;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Details), new { id });
    }
}