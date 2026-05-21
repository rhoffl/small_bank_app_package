using SmallBankApp.Api.Models;

namespace SmallBankApp.Api.Services;

public interface IAmlScreeningService
{
    Task<(bool Allowed, string RiskLevel, string Reason)> ScreenTransferAsync(Guid userId, decimal amount, string memo, CancellationToken ct);
}

public class AmlScreeningService : IAmlScreeningService
{
    public Task<(bool Allowed, string RiskLevel, string Reason)> ScreenTransferAsync(Guid userId, decimal amount, string memo, CancellationToken ct)
    {
        // Replace with vendor screening, sanctions/PEP checks, velocity rules, SAR escalation workflows, and case management.
        if (amount >= 10000m) return Task.FromResult((false, "High", "Amount triggers BSA/AML review threshold workflow."));
        if (memo.Contains("crypto mixer", StringComparison.OrdinalIgnoreCase)) return Task.FromResult((false, "High", "Memo keyword matches elevated-risk typology."));
        if (amount >= 3000m) return Task.FromResult((true, "Medium", "Enhanced monitoring rule."));
        return Task.FromResult((true, "Low", "No rule triggered."));
    }
}
