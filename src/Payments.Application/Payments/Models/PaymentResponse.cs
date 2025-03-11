namespace Payments.Application.Payments.Models;

public record PaymentResponse(
    Guid? ConfirmationCode,
    string ConfirmationMessage,
    PaymentMethod? PaymentMethod,
    DateTime? PaidAt);