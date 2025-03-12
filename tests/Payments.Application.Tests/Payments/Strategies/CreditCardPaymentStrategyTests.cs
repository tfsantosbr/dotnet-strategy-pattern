using Microsoft.Extensions.Logging;
using NSubstitute;
using Payments.Application.Payments.Models;
using Payments.Application.Payments.Strategies;

namespace Payments.Application.Tests.Payments.Strategies;

public class CreditCardPaymentStrategyTests
{
    private readonly CreditCardPaymentStrategy _creditCardPaymentStrategy;

    public CreditCardPaymentStrategyTests()
    {
        var logger = Substitute.For<ILogger<CreditCardPaymentStrategy>>();
        _creditCardPaymentStrategy = new CreditCardPaymentStrategy(logger);
    }

    [Fact]
    public async Task Pay_ShouldThrowArgumentException_WhenRequestIsNotCreditCardPaymentRequest()
    {
        // Arrange
        var invalidRequest = new PixPaymentRequest("valid-pix-key", 100);

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _creditCardPaymentStrategy.Pay(invalidRequest));
            
        // Assert
        Assert.Equal("Invalid request type for Credit Card Payment.", exception.Message);
    }
    
    [Fact]
    public async Task Pay_ShouldReturnError_WhenCardNumberIsNull()
    {
        // Arrange
        var request = new CreditCardPaymentRequest(string.Empty, 12, DateTime.Now.Year + 1, 123, 100);

        // Act
        var response = await _creditCardPaymentStrategy.Pay(request);

        // Assert
        Assert.Null(response.ConfirmationCode);
        Assert.Equal("Card Number is required", response.Message);
    }

    [Fact]
    public async Task Pay_ShouldReturnError_WhenExpirationMonthIsOutOfRange()
    {
        // Arrange
        var request = new CreditCardPaymentRequest("valid-card-number", 0, DateTime.Now.Year + 1, 123, 100);

        // Act
        var response = await _creditCardPaymentStrategy.Pay(request);

        // Assert
        Assert.Null(response.ConfirmationCode);
        Assert.Equal("Expiration Month is out of range", response.Message);
    }

    [Fact]
    public async Task Pay_ShouldReturnError_WhenExpirationYearIsInvalid()
    {
        // Arrange
        var request = new CreditCardPaymentRequest("valid-card-number", 12, DateTime.Now.Year - 1, 123, 100);

        // Act
        var response = await _creditCardPaymentStrategy.Pay(request);

        // Assert
        Assert.Null(response.ConfirmationCode);
        Assert.Equal("Invalid Expiration Year", response.Message);
    }

    [Fact]
    public async Task Pay_ShouldReturnError_WhenCvvIsInvalid()
    {
        // Arrange
        var request = new CreditCardPaymentRequest("valid-card-number", 12, DateTime.Now.Year + 1, 99, 100);

        // Act
        var response = await _creditCardPaymentStrategy.Pay(request);

        // Assert
        Assert.Null(response.ConfirmationCode);
        Assert.Equal("Invalid CVV", response.Message);
    }

    [Fact]
    public async Task Pay_ShouldReturnError_WhenAmountIsNegative()
    {
        // Arrange
        var request = new CreditCardPaymentRequest("valid-card-number", 12, DateTime.Now.Year + 1, 123, -50);

        // Act
        var response = await _creditCardPaymentStrategy.Pay(request);

        // Assert
        Assert.Null(response.ConfirmationCode);
        Assert.Equal("Invalid Amount", response.Message);
    }

    [Fact]
    public async Task Pay_ShouldReturnSuccess_WhenPaymentIsSuccessful()
    {
        // Arrange
        var request = new CreditCardPaymentRequest("valid-card-number", 12, DateTime.Now.Year + 1, 123, 100);

        // Act
        var response = await _creditCardPaymentStrategy.Pay(request);

        // Assert
        Assert.NotNull(response.ConfirmationCode);
        Assert.Equal("Paid with Credit Card Payment", response.Message);
        Assert.Equal(PaymentMethod.CreditCard, response.PaymentMethod);
        Assert.NotNull(response.PaidAt);
    }
}