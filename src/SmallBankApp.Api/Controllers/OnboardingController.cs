using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBankApp.Api.Data;
using SmallBankApp.Api.DTOs;
using SmallBankApp.Api.Models;
using SmallBankApp.Api.Services;
using System.Security.Claims;

namespace SmallBankApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/onboarding")]
public class OnboardingController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IEKycService _ekyc;
    public OnboardingController(AppDbContext db, IEKycService ekyc) { _db = db; _ekyc = ekyc; }
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpPost("ekyc")]
    public async Task<IActionResult> VerifyIdentity(EKycRequest request, CancellationToken ct)
    {
        var user = await _db.Users.FindAsync(new object?[] { CurrentUserId }, ct);
        if (user is null) return NotFound();
        var decision = await _ekyc.VerifyIdentityAsync(request.DocumentType, request.DocumentNumber, request.IssuingState, ct);
        user.KycStatus = decision.Verified ? KycStatus.Verified : KycStatus.ManualReview;
        _db.AuditEvents.Add(new AuditEvent { UserId = user.Id, EventType = "EKYC_DECISION", MetadataJson = $"{{\"decision\":\"{decision.Decision}\",\"reason\":\"{decision.Reason}\"}}" });
        await _db.SaveChangesAsync(ct);
        return Ok(new { user.KycStatus, decision.Decision, decision.Reason });
    }
}
