namespace Payments.Application.Payments.Models;

public record PixPaymentRequest(string Key, decimal Amount)
    : PaymentRequest(Amount);