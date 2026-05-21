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
[Route("api/external-accounts")]
public class ExternalAccountsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ExternalAccountsController(AppDbContext db) => _db = db;
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpPost]
    public async Task<IActionResult> Link(LinkExternalAccountRequest request, CancellationToken ct)
    {
        var masked = $"****{request.AccountNumber[^Math.Min(4, request.AccountNumber.Length)..]}";
        var account = new ExternalAccount { UserId = CurrentUserId, InstitutionName = request.InstitutionName, RoutingNumber = request.RoutingNumber, AccountNumberMasked = masked, IsVerified = false };
        _db.ExternalAccounts.Add(account);
        await _db.SaveChangesAsync(ct);
        return Accepted(new { account.Id, status = "MicroDepositVerificationPending" });
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await _db.ExternalAccounts.Where(x => x.UserId == CurrentUserId).Select(x => new { x.Id, x.InstitutionName, x.AccountNumberMasked, x.IsVerified }).ToListAsync(ct));
}
