using Payments.Application.Payments.Models;

namespace Payments.Application.Payments.Abstractions;

public interface IPaymentStrategyResolver
{
    IPaymentStrategy GetStrategy(PaymentMethod paymentMethod);
}