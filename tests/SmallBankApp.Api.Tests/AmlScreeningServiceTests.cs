using SmallBankApp.Api.Services;
using Xunit;

public class AmlScreeningServiceTests
{
    [Fact]
    public async Task HighAmountTransfer_IsHeldForReview()
    {
        var service = new AmlScreeningService();
        var result = await service.ScreenTransferAsync(Guid.NewGuid(), 10000m, "test", CancellationToken.None);
        Assert.False(result.Allowed);
        Assert.Equal("High", result.RiskLevel);
    }
}
