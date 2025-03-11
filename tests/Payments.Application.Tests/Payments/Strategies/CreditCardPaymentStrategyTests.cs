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
    public async Task Pay_ShouldReturnError_WhenCardNumberIsNull()
    {
        // Arrange
        var request = new CreditCardPaymentRequest(string.Empty, 12, DateTime.Now.Year + 1, 123, 100);

        // Act
        var response = await _creditCardPaymentStrategy.Pay(request);

        // Assert
        Assert.Null(response.ConfirmationCode);
        Assert.Equal("Card Number is required", response.ConfirmationMessage);
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
        Assert.Equal("Expiration Month is out of range", response.ConfirmationMessage);
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
        Assert.Equal("Invalid Expiration Year", response.ConfirmationMessage);
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
        Assert.Equal("Invalid CVV", response.ConfirmationMessage);
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
        Assert.Equal("Invalid Amount", response.ConfirmationMessage);
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
        Assert.Equal("Paid with Credit Card Payment", response.ConfirmationMessage);
        Assert.Equal(request.PaymentMethod, response.PaymentMethod);
        Assert.NotNull(response.PaidAt);
    }
}