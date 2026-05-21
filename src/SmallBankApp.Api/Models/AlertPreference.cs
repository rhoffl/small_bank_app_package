namespace SmallBankApp.Api.Models;

public class AlertPreference
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public bool PushEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; } = true;
    public bool EmailEnabled { get; set; } = true;
    public decimal LowBalanceThreshold { get; set; } = 100m;
    public bool TransactionAlerts { get; set; } = true;
    public bool SecurityAlerts { get; set; } = true;
}
