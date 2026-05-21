namespace SmallBankApp.Api.Models;

public class BankAccount
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public string AccountNumberMasked { get; set; } = string.Empty;
    public string RoutingNumber { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal AvailableBalance { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsFrozen { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

public enum AccountType { Checking, Savings, MoneyMarket }
