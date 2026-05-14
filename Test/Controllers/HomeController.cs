using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Test.Models;

namespace ITSM.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var now = DateTime.UtcNow;
        var vm = new DashboardViewModel
        {
            OpenIncidents = await _db.Incidents.CountAsync(i => i.Status == IncidentStatus.Open),
            InProgressIncidents = await _db.Incidents.CountAsync(i => i.Status == IncidentStatus.InProgress),
            CriticalIncidents = await _db.Incidents.CountAsync(i => i.Priority == Priority.Critical && i.Status != IncidentStatus.Resolved && i.Status != IncidentStatus.Closed),
            OpenProblems = await _db.Problems.CountAsync(p => p.Status != ProblemStatus.Closed),
            PendingChanges = await _db.ChangeRequests.CountAsync(c => c.Status == ChangeStatus.Submitted || c.Status == ChangeStatus.UnderReview),
            OpenServiceRequests = await _db.ServiceRequests.CountAsync(s => s.Status == ServiceRequestStatus.Submitted || s.Status == ServiceRequestStatus.InFulfillment),
            TotalAssets = await _db.Assets.CountAsync(a => a.Status == AssetStatus.InUse),
            KnowledgeArticles = await _db.KnowledgeArticles.CountAsync(k => k.Status == ArticleStatus.Published),
            SLABreached = await _db.Incidents.CountAsync(i => i.SLADueDate < now && i.Status != IncidentStatus.Resolved && i.Status != IncidentStatus.Closed),
            RecentIncidents = await _db.Incidents.OrderByDescending(i => i.CreatedAt).Take(8).ToListAsync(),
            UpcomingChanges = await _db.ChangeRequests.Where(c => c.PlannedStartDate > now && c.Status == ChangeStatus.Approved).OrderBy(c => c.PlannedStartDate).Take(5).ToListAsync(),
        };

        // Incidents by category
        var byCategory = await _db.Incidents.GroupBy(i => i.Category).Select(g => new { Category = g.Key.ToString(), Count = g.Count() }).ToListAsync();
        vm.IncidentsByCategory = byCategory.ToDictionary(x => x.Category, x => x.Count);

        // Incidents by priority
        var byPriority = await _db.Incidents.GroupBy(i => i.Priority).Select(g => new { Priority = g.Key.ToString(), Count = g.Count() }).ToListAsync();
        vm.IncidentsByPriority = byPriority.ToDictionary(x => x.Priority, x => x.Count);

        return View(vm);
    }

    public IActionResult Error() => View();
}