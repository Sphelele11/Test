using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Test.Models;

namespace ITSM.Controllers;

[Authorize]
public class IncidentController : Controller
{
    private readonly ApplicationDbContext _db;

    public IncidentController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? status, string? priority, string? search, int page = 1)
    {
        var query = _db.Incidents.AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<IncidentStatus>(status, out var s))
            query = query.Where(i => i.Status == s);
        if (!string.IsNullOrEmpty(priority) && Enum.TryParse<Priority>(priority, out var p))
            query = query.Where(i => i.Priority == p);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(i => i.Title.Contains(search) || i.TicketNumber.Contains(search) || i.Description.Contains(search));

        int pageSize = 15;
        var total = await query.CountAsync();
        var incidents = await query.OrderByDescending(i => i.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        ViewBag.Total = total;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Status = status;
        ViewBag.Priority = priority;
        ViewBag.Search = search;

        return View(incidents);
    }

    public async Task<IActionResult> Details(int id)
    {
        var incident = await _db.Incidents
            .Include(i => i.Comments)
            .Include(i => i.Attachments)
            .Include(i => i.LinkedProblem)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (incident == null) return NotFound();
        return View(incident);
    }

    public IActionResult Create() => View(new Incident { SLADueDate = DateTime.UtcNow.AddHours(8) });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Incident incident)
    {
        if (!ModelState.IsValid) return View(incident);

        incident.TicketNumber = await GenerateTicketNumber("INC");
        incident.CreatedAt = DateTime.UtcNow;
        incident.UpdatedAt = DateTime.UtcNow;

        // Auto-set SLA based on priority
        incident.SLADueDate = incident.Priority switch
        {
            Priority.Critical => DateTime.UtcNow.AddHours(1),
            Priority.High => DateTime.UtcNow.AddHours(4),
            Priority.Medium => DateTime.UtcNow.AddHours(8),
            _ => DateTime.UtcNow.AddHours(24)
        };

        _db.Incidents.Add(incident);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Incident {incident.TicketNumber} created successfully.";
        return RedirectToAction(nameof(Details), new { id = incident.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var incident = await _db.Incidents.FindAsync(id);
        if (incident == null) return NotFound();
        return View(incident);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Incident incident)
    {
        if (id != incident.Id) return BadRequest();
        if (!ModelState.IsValid) return View(incident);

        var existing = await _db.Incidents.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Title = incident.Title;
        existing.Description = incident.Description;
        existing.Priority = incident.Priority;
        existing.Status = incident.Status;
        existing.Category = incident.Category;
        existing.AssignedTo = incident.AssignedTo;
        existing.AffectedSystem = incident.AffectedSystem;
        existing.ResolutionNotes = incident.ResolutionNotes;
        existing.UpdatedAt = DateTime.UtcNow;

        if (incident.Status == IncidentStatus.Resolved && existing.ResolvedAt == null)
            existing.ResolvedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Incident updated successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int id, string content, bool isInternal)
    {
        var comment = new Comment
        {
            IncidentId = id,
            Content = content,
            Author = User.Identity?.Name ?? "Unknown",
            IsInternal = isInternal,
            CreatedAt = DateTime.UtcNow
        };
        _db.Comments.Add(comment);

        var incident = await _db.Incidents.FindAsync(id);
        if (incident != null) incident.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var incident = await _db.Incidents.FindAsync(id);
        if (incident != null)
        {
            _db.Incidents.Remove(incident);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Incident {incident.TicketNumber} deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<string> GenerateTicketNumber(string prefix)
    {
        var count = await _db.Incidents.CountAsync();
        return $"{prefix}-{(count + 1):D4}";
    }
}