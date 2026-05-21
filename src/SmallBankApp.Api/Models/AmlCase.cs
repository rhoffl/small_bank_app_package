namespace SmallBankApp.Api.Models;

public class AmlCase
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? TransactionId { get; set; }
    public string RuleCode { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = "Low";
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
