using Payments.Application.Payments.Models;

namespace Payments.Api.Models;

public record PayOrderRequest(
    Guid OrderId,
    PaymentMethod PaymentMethod,
    PixPaymentData? PixPaymentData,
    BoletoPaymentData? BoletoPaymentData,
    CreditCardPaymentData? CreditCardPaymentData,
    decimal Amount
)
{
    public PaymentRequest ToPaymentRequest() => PaymentMethod switch
    {
        PaymentMethod.Pix => new PixPaymentRequest(PixPaymentData!.Key, Amount),
        PaymentMethod.Boleto =>
            new BoletoPaymentRequest(BoletoPaymentData!.DueDate, BoletoPaymentData!.BarCode, Amount),
        PaymentMethod.CreditCard => new CreditCardPaymentRequest(
            CreditCardPaymentData!.CardNumber,
            CreditCardPaymentData!.ExpirationMonth,
            CreditCardPaymentData!.ExpirationYear,
            CreditCardPaymentData!.Cvv,
            Amount),
        _ => throw new ArgumentException("Invalid payment method")
    };
}