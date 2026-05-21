namespace SmallBankApp.Api.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public BankAccount? Account { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Description { get; set; } = string.Empty;
    public string MerchantCategoryCode { get; set; } = string.Empty;
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public DateTimeOffset PostedAt { get; set; } = DateTimeOffset.UtcNow;
    public string ExternalReference { get; set; } = string.Empty;
}

public enum TransactionType { Debit, Credit, TransferOut, TransferIn, BillPay, Fee, Interest }
public enum TransactionStatus { Pending, Posted, Reversed, Failed, HeldForReview }
