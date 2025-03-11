namespace Payments.Application.Payments.Models;

public abstract record PaymentRequest(decimal Amount, PaymentMethod PaymentMethod);