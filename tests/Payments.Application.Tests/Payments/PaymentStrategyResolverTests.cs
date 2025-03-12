using NSubstitute;
using Payments.Application.Payments;
using Payments.Application.Payments.Abstractions;
using Payments.Application.Payments.Models;

namespace Payments.Application.Tests.Payments
{
    public class PaymentStrategyResolverTests
    {
        private readonly IPaymentStrategyResolver _resolver;
        private readonly IPaymentStrategy _mockCreditCardStrategy;
        private readonly IPaymentStrategy _mockPixStrategy;
        private readonly IPaymentStrategy _mockBoletoStrategy;

        public PaymentStrategyResolverTests()
        {
            _mockCreditCardStrategy = Substitute.For<IPaymentStrategy>();
            _mockCreditCardStrategy.PaymentMethod.Returns(PaymentMethod.CreditCard);

            _mockPixStrategy = Substitute.For<IPaymentStrategy>();
            _mockPixStrategy.PaymentMethod.Returns(PaymentMethod.Pix);

            _mockBoletoStrategy = Substitute.For<IPaymentStrategy>();
            _mockBoletoStrategy.PaymentMethod.Returns(PaymentMethod.Boleto);

            var strategies = new List<IPaymentStrategy>
            {
                _mockCreditCardStrategy,
                _mockPixStrategy,
                _mockBoletoStrategy
            };

            _resolver = new PaymentStrategyResolver(strategies);
        }

        [Fact]
        public void GetStrategy_ShouldReturnCreditCardStrategy_WhenPaymentMethodIsCreditCard()
        {
            // Arrange
            const PaymentMethod paymentMethod = PaymentMethod.CreditCard;

            // Act
            var strategy = _resolver.GetStrategy(paymentMethod);

            // Assert
            Assert.Equal(_mockCreditCardStrategy, strategy);
        }

        [Fact]
        public void GetStrategy_ShouldReturnPixStrategy_WhenPaymentMethodIsPix()
        {
            // Arrange
            const PaymentMethod paymentMethod = PaymentMethod.Pix;

            // Act
            var strategy = _resolver.GetStrategy(paymentMethod);

            // Assert
            Assert.Equal(_mockPixStrategy, strategy);
        }

        [Fact]
        public void GetStrategy_ShouldReturnBoletoStrategy_WhenPaymentMethodIsBoleto()
        {
            // Arrange
            const PaymentMethod paymentMethod = PaymentMethod.Boleto;

            // Act
            var strategy = _resolver.GetStrategy(paymentMethod);

            // Assert
            Assert.Equal(_mockBoletoStrategy, strategy);
        }

        [Fact]
        public void GetStrategy_ShouldThrowArgumentException_WhenPaymentMethodIsUnknown()
        {
            // Arrange
            const PaymentMethod paymentMethod = (PaymentMethod)999;

            // Act
            var exception = Assert.Throws<ArgumentException>(() => _resolver.GetStrategy(paymentMethod));
            
            // Assert
            Assert.Equal($"No strategy found for payment method '{paymentMethod}'.", exception.Message);
        }
    }
}