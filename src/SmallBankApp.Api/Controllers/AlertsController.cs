using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBankApp.Api.Data;
using SmallBankApp.Api.DTOs;
using SmallBankApp.Api.Models;
using System.Security.Claims;

namespace SmallBankApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/alerts")]
public class AlertsController : ControllerBase
{
    private readonly AppDbContext _db;
    public AlertsController(AppDbContext db) => _db = db;
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpGet("preferences")]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var prefs = await _db.AlertPreferences.FirstOrDefaultAsync(x => x.UserId == CurrentUserId, ct)
                    ?? new AlertPreference { UserId = CurrentUserId };
        return Ok(prefs);
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> Save(AlertPreferencesRequest request, CancellationToken ct)
    {
        var prefs = await _db.AlertPreferences.FirstOrDefaultAsync(x => x.UserId == CurrentUserId, ct);
        if (prefs is null) { prefs = new AlertPreference { UserId = CurrentUserId }; _db.AlertPreferences.Add(prefs); }
        prefs.PushEnabled = request.PushEnabled; prefs.SmsEnabled = request.SmsEnabled; prefs.EmailEnabled = request.EmailEnabled;
        prefs.LowBalanceThreshold = request.LowBalanceThreshold; prefs.TransactionAlerts = request.TransactionAlerts; prefs.SecurityAlerts = request.SecurityAlerts;
        await _db.SaveChangesAsync(ct);
        return Ok(prefs);
    }
}
