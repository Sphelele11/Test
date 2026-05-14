using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Test.Data;
using Test.Models;

[Authorize]
public class ServiceRequestController : Controller
{
    private readonly ApplicationDbContext _db;
    public ServiceRequestController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? status, string? search)
    {
        var query = _db.ServiceRequests.AsQueryable();
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ServiceRequestStatus>(status, out var s))
            query = query.Where(r => r.Status == s);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(r => r.Title.Contains(search) || r.TicketNumber.Contains(search));
        return View(await query.OrderByDescending(r => r.CreatedAt).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var sr = await _db.ServiceRequests.Include(s => s.Comments).FirstOrDefaultAsync(s => s.Id == id);
        if (sr == null) return NotFound();
        return View(sr);
    }

    public IActionResult Create() => View(new ServiceRequest());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceRequest sr)
    {
        if (!ModelState.IsValid) return View(sr);
        sr.TicketNumber = $"SRQ-{(await _db.ServiceRequests.CountAsync() + 1):D4}";
        sr.RequestedBy = User.Identity?.Name;
        sr.CreatedAt = sr.UpdatedAt = DateTime.UtcNow;
        sr.SLADueDate = DateTime.UtcNow.AddDays(5);
        _db.ServiceRequests.Add(sr);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Service request {sr.TicketNumber} submitted.";
        return RedirectToAction(nameof(Details), new { id = sr.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var sr = await _db.ServiceRequests.FindAsync(id);
        if (sr == null) return NotFound();
        return View(sr);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceRequest sr)
    {
        if (!ModelState.IsValid) return View(sr);
        var existing = await _db.ServiceRequests.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Title = sr.Title; existing.Description = sr.Description;
        existing.Status = sr.Status; existing.Category = sr.Category; existing.Priority = sr.Priority;
        existing.AssignedTo = sr.AssignedTo; existing.Department = sr.Department;
        existing.UpdatedAt = DateTime.UtcNow;
        if (sr.Status == ServiceRequestStatus.Completed) existing.CompletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Service request updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int id, string content, bool isInternal)
    {
        _db.Comments.Add(new Comment { ServiceRequestId = id, Content = content, Author = User.Identity?.Name ?? "Unknown", IsInternal = isInternal, CreatedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }
}