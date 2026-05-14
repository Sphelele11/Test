using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Test.Data;
using Test.Models;

[Authorize]
public class ChangeController : Controller
{
    private readonly ApplicationDbContext _db;
    public ChangeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? status, string? search)
    {
        var query = _db.ChangeRequests.AsQueryable();
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ChangeStatus>(status, out var s))
            query = query.Where(c => c.Status == s);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(c => c.Title.Contains(search) || c.TicketNumber.Contains(search));
        return View(await query.OrderByDescending(c => c.CreatedAt).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var change = await _db.ChangeRequests.Include(c => c.Comments).FirstOrDefaultAsync(c => c.Id == id);
        if (change == null) return NotFound();
        return View(change);
    }

    public IActionResult Create() => View(new ChangeRequest { PlannedStartDate = DateTime.UtcNow.AddDays(7), PlannedEndDate = DateTime.UtcNow.AddDays(7).AddHours(4) });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ChangeRequest change)
    {
        if (!ModelState.IsValid) return View(change);
        change.TicketNumber = $"CHG-{(await _db.ChangeRequests.CountAsync() + 1):D4}";
        change.RequestedBy = User.Identity?.Name;
        change.CreatedAt = change.UpdatedAt = DateTime.UtcNow;
        _db.ChangeRequests.Add(change);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Change request {change.TicketNumber} created.";
        return RedirectToAction(nameof(Details), new { id = change.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var c = await _db.ChangeRequests.FindAsync(id);
        if (c == null) return NotFound();
        return View(c);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ChangeRequest change)
    {
        if (!ModelState.IsValid) return View(change);
        var existing = await _db.ChangeRequests.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Title = change.Title; existing.Description = change.Description;
        existing.Type = change.Type; existing.Status = change.Status; existing.Risk = change.Risk;
        existing.AssignedTo = change.AssignedTo; existing.Justification = change.Justification;
        existing.ImplementationPlan = change.ImplementationPlan; existing.BackoutPlan = change.BackoutPlan;
        existing.TestPlan = change.TestPlan; existing.PlannedStartDate = change.PlannedStartDate;
        existing.PlannedEndDate = change.PlannedEndDate; existing.UpdatedAt = DateTime.UtcNow;
        if (change.Status == ChangeStatus.Approved) { existing.ApprovedBy = User.Identity?.Name; existing.ApprovedAt = DateTime.UtcNow; }
        await _db.SaveChangesAsync();
        TempData["Success"] = "Change request updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int id, string content, bool isInternal)
    {
        _db.Comments.Add(new Comment { ChangeRequestId = id, Content = content, Author = User.Identity?.Name ?? "Unknown", IsInternal = isInternal, CreatedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }
}