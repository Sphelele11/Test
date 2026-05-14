using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Test.Models;

namespace ITSM.Controllers;

// ============================================================
// Problem Controller
// ============================================================
[Authorize]
public class ProblemController : Controller
{
    private readonly ApplicationDbContext _db;
    public ProblemController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? status, string? search)
    {
        var query = _db.Problems.AsQueryable();
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ProblemStatus>(status, out var s))
            query = query.Where(p => p.Status == s);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Title.Contains(search) || p.TicketNumber.Contains(search));
        return View(await query.OrderByDescending(p => p.CreatedAt).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var problem = await _db.Problems.Include(p => p.Comments).Include(p => p.RelatedIncidents).FirstOrDefaultAsync(p => p.Id == id);
        if (problem == null) return NotFound();
        return View(problem);
    }

    public IActionResult Create() => View(new Problem());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Problem problem)
    {
        if (!ModelState.IsValid) return View(problem);
        problem.TicketNumber = $"PRB-{(await _db.Problems.CountAsync() + 1):D4}";
        problem.CreatedAt = problem.UpdatedAt = DateTime.UtcNow;
        _db.Problems.Add(problem);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Problem {problem.TicketNumber} created.";
        return RedirectToAction(nameof(Details), new { id = problem.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var p = await _db.Problems.FindAsync(id);
        if (p == null) return NotFound();
        return View(p);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Problem problem)
    {
        if (!ModelState.IsValid) return View(problem);
        var existing = await _db.Problems.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Title = problem.Title; existing.Description = problem.Description;
        existing.Priority = problem.Priority; existing.Status = problem.Status;
        existing.AssignedTo = problem.AssignedTo; existing.RootCause = problem.RootCause;
        existing.Workaround = problem.Workaround; existing.PermanentFix = problem.PermanentFix;
        existing.UpdatedAt = DateTime.UtcNow;
        if (problem.Status == ProblemStatus.Resolved) existing.ResolvedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Problem updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int id, string content, bool isInternal)
    {
        _db.Comments.Add(new Comment { ProblemId = id, Content = content, Author = User.Identity?.Name ?? "Unknown", IsInternal = isInternal, CreatedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }
}
