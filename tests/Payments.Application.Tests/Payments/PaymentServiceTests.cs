using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Payments.Application.Payments;
using Payments.Application.Payments.Abstractions;
using Payments.Application.Payments.Models;

namespace Payments.Application.Tests.Payments;

public class PaymentServiceTests
{
    private readonly IPaymentStrategyResolver _resolver;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _resolver = Substitute.For<IPaymentStrategyResolver>();
        _paymentService = new PaymentService(_resolver);
    }

    [Fact]
    public async Task ProcessPayment_ShouldReturnPaymentResponse_WhenPaymentIsSuccessful()
    {
        // Arrange
        var request = new CreditCardPaymentRequest("valid-card-number", 12, DateTime.Now.Year + 1, 123, 100);
        
        var expectedResponse = new PaymentResponse(
            ConfirmationCode: Guid.NewGuid(),
            ConfirmationMessage: "Paid with Credit Card Payment", 
            PaymentMethod: PaymentMethod.CreditCard,
            PaidAt: DateTime.UtcNow);

        var strategy = Substitute.For<IPaymentStrategy>();
        
        strategy.Pay(request).Returns(expectedResponse);
        
        _resolver.GetStrategy(PaymentMethod.CreditCard).Returns(strategy);

        // Act
        var response = await _paymentService.ProcessPayment(request, PaymentMethod.CreditCard);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(expectedResponse.ConfirmationMessage, response.ConfirmationMessage);
        Assert.Equal(expectedResponse.PaymentMethod, response.PaymentMethod);
    }

    [Fact]
    public async Task ProcessPayment_ShouldThrowArgumentException_WhenInvalidPaymentMethod()
    {
        // Arrange
        var request = new CreditCardPaymentRequest("valid-card-number", 12, DateTime.Now.Year + 1, 123, 100);
        
        _resolver.GetStrategy(Arg.Any<PaymentMethod>()).Throws(new ArgumentException("Invalid payment method"));

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _paymentService.ProcessPayment(request, PaymentMethod.CreditCard)
            );
        
        // Assert
        Assert.Equal("Invalid payment method", exception.Message);
    }
}