using Microsoft.Extensions.Logging;
using NSubstitute;
using Payments.Application.Payments.Models;
using Payments.Application.Payments.Strategies;
using Xunit;

namespace Payments.Application.Tests.Payments.Strategies
{
    public class BoletoPaymentStrategyTests
    {
        private readonly BoletoPaymentStrategy _strategy;

        public BoletoPaymentStrategyTests()
        {
            var logger = Substitute.For<ILogger<BoletoPaymentStrategy>>();
            _strategy = new BoletoPaymentStrategy(logger);
        }

        [Fact]
        public async Task Pay_ShouldThrowArgumentException_WhenRequestIsNotBoletoPaymentRequest()
        {
            // Arrange
            var invalidRequest = new PixPaymentRequest("valid-pix-key", 100);

            // Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _strategy.Pay(invalidRequest));
            
            // Assert
            Assert.Equal("Invalid request type for Boleto Payment.", exception.Message);
        }

        [Fact]
        public async Task Pay_ShouldReturnError_WhenBarCodeIsNullOrEmpty()
        {
            var request = new BoletoPaymentRequest(DateTime.UtcNow.AddDays(1), "", 100);

            var response = await _strategy.Pay(request);

            Assert.Null(response.ConfirmationCode);
            Assert.Equal("Barcode is required", response.Message);
        }

        [Fact]
        public async Task Pay_ShouldReturnError_WhenDueDateIsInThePast()
        {
            var request = new BoletoPaymentRequest(DateTime.UtcNow.AddDays(-1), "123456789", 100);

            var response = await _strategy.Pay(request);

            Assert.Null(response.ConfirmationCode);
            Assert.Equal("Invalid Due Date", response.Message);
        }

        [Fact]
        public async Task Pay_ShouldReturnError_WhenAmountIsNegative()
        {
            var request = new BoletoPaymentRequest(DateTime.UtcNow.AddDays(1), "123456789", -100);

            var response = await _strategy.Pay(request);

            Assert.Null(response.ConfirmationCode);
            Assert.Equal("Invalid Amount", response.Message);
        }

        [Fact]
        public async Task Pay_ShouldReturnSuccess_WhenRequestIsValid()
        {
            var request = new BoletoPaymentRequest(DateTime.UtcNow.AddDays(1), "123456789", 100);

            var response = await _strategy.Pay(request);

            Assert.NotNull(response.ConfirmationCode);
            Assert.Equal("Paid with Boleto Payment", response.Message);
            Assert.Equal(PaymentMethod.Boleto, response.PaymentMethod);
            Assert.NotNull(response.PaidAt);
        }
    }
}