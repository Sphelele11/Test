using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Test.Data;
using Test.Models;

[Authorize]
public class AssetController : Controller
{
    private readonly ApplicationDbContext _db;
    public AssetController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? type, string? status, string? search)
    {
        var query = _db.Assets.AsQueryable();
        if (!string.IsNullOrEmpty(type) && Enum.TryParse<AssetType>(type, out var t))
            query = query.Where(a => a.Type == t);
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<AssetStatus>(status, out var s))
            query = query.Where(a => a.Status == s);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(a => a.Name.Contains(search) || a.AssetTag.Contains(search) || (a.SerialNumber != null && a.SerialNumber.Contains(search)));
        return View(await query.OrderBy(a => a.AssetTag).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var asset = await _db.Assets.FindAsync(id);
        if (asset == null) return NotFound();
        return View(asset);
    }

    public IActionResult Create() => View(new Asset());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Asset asset)
    {
        if (!ModelState.IsValid) return View(asset);
        var count = await _db.Assets.CountAsync();
        asset.AssetTag = $"AST-{(count + 1):D3}";
        asset.CreatedAt = asset.UpdatedAt = DateTime.UtcNow;
        _db.Assets.Add(asset);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Asset {asset.AssetTag} created.";
        return RedirectToAction(nameof(Details), new { id = asset.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var a = await _db.Assets.FindAsync(id);
        if (a == null) return NotFound();
        return View(a);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Asset asset)
    {
        if (!ModelState.IsValid) return View(asset);
        var existing = await _db.Assets.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Name = asset.Name; existing.Type = asset.Type; existing.Status = asset.Status;
        existing.Manufacturer = asset.Manufacturer; existing.Model = asset.Model;
        existing.SerialNumber = asset.SerialNumber; existing.Location = asset.Location;
        existing.AssignedTo = asset.AssignedTo; existing.Department = asset.Department;
        existing.IpAddress = asset.IpAddress; existing.OperatingSystem = asset.OperatingSystem;
        existing.PurchaseCost = asset.PurchaseCost; existing.PurchaseDate = asset.PurchaseDate;
        existing.WarrantyExpiry = asset.WarrantyExpiry; existing.Notes = asset.Notes;
        existing.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Asset updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var asset = await _db.Assets.FindAsync(id);
        if (asset != null) { _db.Assets.Remove(asset); await _db.SaveChangesAsync(); TempData["Success"] = $"Asset {asset.AssetTag} deleted."; }
        return RedirectToAction(nameof(Index));
    }
}