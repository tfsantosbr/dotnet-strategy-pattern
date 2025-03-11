namespace Payments.Application.Payments.Models;

public record CreditCardPaymentRequest(
    string CardNumber,
    int ExpirationMonth,
    int ExpirationYear,
    int Cvv,
    decimal Amount)
    : PaymentRequest(Amount, PaymentMethod.CreditCard);