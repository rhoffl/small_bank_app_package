namespace SmallBankApp.Api.Services;

public interface IAlertService
{
    Task SendTransactionAlertAsync(Guid userId, string message, CancellationToken ct);
    Task SendSecurityAlertAsync(Guid userId, string message, CancellationToken ct);
}

public class AlertService : IAlertService
{
    private readonly ILogger<AlertService> _logger;
    public AlertService(ILogger<AlertService> logger) => _logger = logger;
    public Task SendTransactionAlertAsync(Guid userId, string message, CancellationToken ct)
    {
        _logger.LogInformation("Transaction alert for {UserId}: {Message}", userId, message);
        return Task.CompletedTask;
    }
    public Task SendSecurityAlertAsync(Guid userId, string message, CancellationToken ct)
    {
        _logger.LogWarning("Security alert for {UserId}: {Message}", userId, message);
        return Task.CompletedTask;
    }
}
