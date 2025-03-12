namespace Payments.Application.Payments.Models;

public record PaymentResponse(
    Guid? ConfirmationCode,
    string Message,
    PaymentMethod? PaymentMethod,
    DateTime? PaidAt);