using Microsoft.Extensions.Logging;
using NSubstitute;
using Payments.Application.Payments.Models;
using Payments.Application.Payments.Strategies;

namespace Payments.Application.Tests.Payments.Strategies;

public class PixPaymentStrategyTests
{
    private readonly PixPaymentStrategy _pixPaymentStrategy;

    public PixPaymentStrategyTests()
    {
        var logger = Substitute.For<ILogger<PixPaymentStrategy>>();
        _pixPaymentStrategy = new PixPaymentStrategy(logger);
    }

    [Fact]
    public async Task Pay_ShouldReturnError_WhenKeyIsNull()
    {
        // Arrange
        var request = new PixPaymentRequest(string.Empty, 100);

        // Act
        var response = await _pixPaymentStrategy.Pay(request);

        // Assert
        Assert.Null(response.ConfirmationCode);
        Assert.Equal("Key is required", response.ConfirmationMessage);
    }

    [Fact]
    public async Task Pay_ShouldReturnError_WhenAmountIsNegative()
    {
        // Arrange
        var request = new PixPaymentRequest("valid-key", -50);

        // Act
        var response = await _pixPaymentStrategy.Pay(request);

        // Assert
        Assert.Null(response.ConfirmationCode);
        Assert.Equal("Invalid Amount", response.ConfirmationMessage);
    }

    [Fact]
    public async Task Pay_ShouldReturnSuccess_WhenPaymentIsSuccessful()
    {
        // Arrange
        var request = new PixPaymentRequest("valid-key", 100);

        // Act
        var response = await _pixPaymentStrategy.Pay(request);

        // Assert
        Assert.NotNull(response.ConfirmationCode);
        Assert.Equal("Paid with Pix Payment", response.ConfirmationMessage);
        Assert.Equal(PaymentMethod.Pix, response.PaymentMethod);
        Assert.NotNull(response.PaidAt);
    }
}