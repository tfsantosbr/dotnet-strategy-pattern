using Payments.Application.Payments.Abstractions;
using Payments.Application.Payments.Models;

namespace Payments.Application.Payments;

public class PaymentStrategyResolver : IPaymentStrategyResolver
{
    private readonly Dictionary<PaymentMethod, IPaymentStrategy> _strategies;

    public PaymentStrategyResolver(IEnumerable<IPaymentStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(strategy => strategy.PaymentMethod);
    }
    
    public IPaymentStrategy GetStrategy(PaymentMethod paymentMethod)
    {
        if (_strategies.TryGetValue(paymentMethod, out var strategy))
            return strategy;

        throw new ArgumentException($"No strategy found for payment method '{paymentMethod}'.");
    }
}