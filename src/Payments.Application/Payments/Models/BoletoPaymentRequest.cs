namespace Payments.Application.Payments.Models;

public record BoletoPaymentRequest(DateTime DueDate, string BarCode, decimal Amount)
    : PaymentRequest(Amount, PaymentMethod.Boleto);