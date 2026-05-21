namespace SmallBankApp.Api.Services;

public interface IPaymentGatewayService
{
    Task<string> SubmitExternalTransferAsync(decimal amount, string routingNumber, string maskedAccount, CancellationToken ct);
}

public class PaymentGatewayService : IPaymentGatewayService
{
    public Task<string> SubmitExternalTransferAsync(decimal amount, string routingNumber, string maskedAccount, CancellationToken ct)
    {
        return Task.FromResult($"EXT-{Guid.NewGuid():N}");
    }
}
