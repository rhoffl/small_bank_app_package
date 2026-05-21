using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmallBankApp.Api.Models;

namespace SmallBankApp.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<ExternalAccount> ExternalAccounts => Set<ExternalAccount>();
    public DbSet<AlertPreference> AlertPreferences => Set<AlertPreference>();
    public DbSet<AmlCase> AmlCases => Set<AmlCase>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<BankAccount>().Property(x => x.CurrentBalance).HasPrecision(18, 2);
        builder.Entity<BankAccount>().Property(x => x.AvailableBalance).HasPrecision(18, 2);
        builder.Entity<Transaction>().Property(x => x.Amount).HasPrecision(18, 2);
        builder.Entity<AlertPreference>().Property(x => x.LowBalanceThreshold).HasPrecision(18, 2);
        builder.Entity<BankAccount>().HasIndex(x => new { x.UserId, x.AccountNumberMasked });
        builder.Entity<Transaction>().HasIndex(x => new { x.AccountId, x.PostedAt });
        builder.Entity<AuditEvent>().HasIndex(x => x.CreatedAt);
    }
}
