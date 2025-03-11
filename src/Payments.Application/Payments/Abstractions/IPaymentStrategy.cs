using Payments.Application.Payments.Models;

namespace Payments.Application.Payments.Abstractions;

public interface IPaymentStrategy<in TRequest>
    where TRequest : PaymentRequest 
{
    Task<PaymentResponse> Pay(TRequest request);
}   