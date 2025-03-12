using Payments.Application.Payments.Models;

namespace Payments.Application.Payments.Abstractions;

public interface IPaymentStrategy
{
    Task<PaymentResponse> Pay(PaymentRequest request);
}
