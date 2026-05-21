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
[Route("api/transfers")]
public class TransfersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAmlScreeningService _aml;
    private readonly IAlertService _alerts;
    private readonly IPaymentGatewayService _payments;
    public TransfersController(AppDbContext db, IAmlScreeningService aml, IAlertService alerts, IPaymentGatewayService payments)
    { _db = db; _aml = aml; _alerts = alerts; _payments = payments; }
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpPost]
    public async Task<IActionResult> CreateTransfer(TransferRequest request, CancellationToken ct)
    {
        if (request.Amount <= 0) return BadRequest("Amount must be positive.");
        var from = await _db.BankAccounts.FirstOrDefaultAsync(a => a.Id == request.FromAccountId && a.UserId == CurrentUserId, ct);
        if (from is null || from.IsFrozen) return BadRequest("Account unavailable.");
        if (from.AvailableBalance < request.Amount) return BadRequest("Insufficient funds.");

        var screen = await _aml.ScreenTransferAsync(CurrentUserId, request.Amount, request.Memo, ct);
        if (!screen.Allowed)
        {
            var amlCase = new AmlCase { UserId = CurrentUserId, RiskLevel = screen.RiskLevel, Reason = screen.Reason, RuleCode = "AML-TRANSFER-REVIEW" };
            _db.AmlCases.Add(amlCase);
            await _db.SaveChangesAsync(ct);
            return Accepted(new { status = "HeldForReview", screen.RiskLevel, screen.Reason, caseId = amlCase.Id });
        }

        from.AvailableBalance -= request.Amount;
        from.CurrentBalance -= request.Amount;
        var outTxn = new Transaction { AccountId = from.Id, Amount = request.Amount, Type = TransactionType.TransferOut, Status = TransactionStatus.Posted, Description = request.Memo };
        _db.Transactions.Add(outTxn);

        if (request.ToInternalAccountId is Guid toId)
        {
            var to = await _db.BankAccounts.FirstOrDefaultAsync(a => a.Id == toId, ct);
            if (to is null) return BadRequest("Destination account not found.");
            to.AvailableBalance += request.Amount;
            to.CurrentBalance += request.Amount;
            _db.Transactions.Add(new Transaction { AccountId = to.Id, Amount = request.Amount, Type = TransactionType.TransferIn, Status = TransactionStatus.Posted, Description = request.Memo });
        }
        else if (request.ToExternalAccountId is Guid extId)
        {
            var ext = await _db.ExternalAccounts.FirstOrDefaultAsync(e => e.Id == extId && e.UserId == CurrentUserId, ct);
            if (ext is null || !ext.IsVerified) return BadRequest("External account unavailable.");
            outTxn.ExternalReference = await _payments.SubmitExternalTransferAsync(request.Amount, ext.RoutingNumber, ext.AccountNumberMasked, ct);
        }
        await _db.SaveChangesAsync(ct);
        await _alerts.SendTransactionAlertAsync(CurrentUserId, $"Transfer of {request.Amount:C} completed.", ct);
        return Ok(new { status = "Posted", transactionId = outTxn.Id, screen.RiskLevel });
    }
}
