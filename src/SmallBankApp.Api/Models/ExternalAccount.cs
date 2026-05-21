namespace SmallBankApp.Api.Models;

public class ExternalAccount
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string InstitutionName { get; set; } = string.Empty;
    public string AccountNumberMasked { get; set; } = string.Empty;
    public string RoutingNumber { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
