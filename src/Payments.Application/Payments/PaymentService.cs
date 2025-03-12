using Payments.Application.Payments.Abstractions;
using Payments.Application.Payments.Models;

namespace Payments.Application.Payments;

public class PaymentService(IPaymentStrategyResolver resolver) : IPaymentService
{
    public Task<PaymentResponse> ProcessPayment(PaymentRequest request, PaymentMethod method)
    {
        var strategy = resolver.GetStrategy(method);
        
        return strategy.Pay(request);
    }
}