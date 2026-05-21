using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBankApp.Api.Data;
using System.Security.Claims;

namespace SmallBankApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly AppDbContext _db;
    public AccountsController(AppDbContext db) => _db = db;
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<IActionResult> GetAccounts(CancellationToken ct)
    {
        var accounts = await _db.BankAccounts.Where(a => a.UserId == CurrentUserId)
            .Select(a => new { a.Id, a.AccountType, a.AccountNumberMasked, a.AvailableBalance, a.CurrentBalance, a.Currency, a.IsFrozen })
            .ToListAsync(ct);
        return Ok(accounts);
    }

    [HttpGet("{accountId:guid}/transactions")]
    public async Task<IActionResult> GetTransactions(Guid accountId, CancellationToken ct)
    {
        var txns = await _db.Transactions.Where(t => t.AccountId == accountId && t.Account!.UserId == CurrentUserId)
            .OrderByDescending(t => t.PostedAt).Take(100)
            .Select(t => new { t.Id, t.Type, t.Amount, t.Currency, t.Description, t.Status, t.PostedAt })
            .ToListAsync(ct);
        return Ok(txns);
    }

    [HttpGet("{accountId:guid}/statement")]
    public async Task<IActionResult> GetStatement(Guid accountId, CancellationToken ct)
    {
        var account = await _db.BankAccounts.FirstOrDefaultAsync(a => a.Id == accountId && a.UserId == CurrentUserId, ct);
        if (account is null) return NotFound();
        return Ok(new { account.Id, account.AccountNumberMasked, statementMonth = DateTime.UtcNow.ToString("yyyy-MM"), message = "PDF e-statement generation endpoint stub." });
    }
}
