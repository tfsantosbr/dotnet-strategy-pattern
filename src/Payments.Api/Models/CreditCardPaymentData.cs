namespace Payments.Api.Models;

public record CreditCardPaymentData(string CardNumber, int ExpirationMonth, int ExpirationYear, int Cvv);