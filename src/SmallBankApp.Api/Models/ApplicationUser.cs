using Microsoft.AspNetCore.Identity;

namespace SmallBankApp.Api.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public string LegalFirstName { get; set; } = string.Empty;
    public string LegalLastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public KycStatus KycStatus { get; set; } = KycStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<BankAccount> Accounts { get; set; } = new List<BankAccount>();
}

public enum KycStatus { Pending, Verified, Rejected, ManualReview }
