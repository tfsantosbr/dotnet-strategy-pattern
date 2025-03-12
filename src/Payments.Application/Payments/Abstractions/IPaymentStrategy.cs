using Payments.Application.Payments.Models;

namespace Payments.Application.Payments.Abstractions;

public interface IPaymentStrategy
{
    PaymentMethod PaymentMethod { get; }
    
    Task<PaymentResponse> Pay(PaymentRequest request);
}
