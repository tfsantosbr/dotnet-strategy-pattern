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
    public async Task Pay_ShouldThrowArgumentException_WhenRequestIsNotPixPaymentRequest()
    {
        // Arrange
        var invalidRequest = new BoletoPaymentRequest(DateTime.UtcNow, "valid-bar-code", 100);

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _pixPaymentStrategy.Pay(invalidRequest));

        // Assert
        Assert.Equal("Invalid request type for Pix Payment.", exception.Message);
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
        Assert.Equal("Key is required", response.Message);
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
        Assert.Equal("Invalid Amount", response.Message);
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
        Assert.Equal("Paid with Pix Payment", response.Message);
        Assert.Equal(PaymentMethod.Pix, response.PaymentMethod);
        Assert.NotNull(response.PaidAt);
    }
}