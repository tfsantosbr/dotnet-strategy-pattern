using Payments.Application.Payments.Models;

namespace Payments.Application.Payments.Abstractions;

public interface IPaymentService
{
    Task<PaymentResponse> ProcessPayment(PaymentRequest request, PaymentMethod method);
}