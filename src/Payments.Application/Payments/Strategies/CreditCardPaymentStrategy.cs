using Microsoft.Extensions.Logging;
using Payments.Application.Payments.Abstractions;
using Payments.Application.Payments.Models;

namespace Payments.Application.Payments.Strategies;

public class CreditCardPaymentStrategy(ILogger<CreditCardPaymentStrategy> logger) : IPaymentStrategy
{
    public PaymentMethod PaymentMethod => PaymentMethod.CreditCard;

    public Task<PaymentResponse> Pay(PaymentRequest request)
    {
        var creditCardPaymentRequest = request as CreditCardPaymentRequest
                                       ?? throw new ArgumentException("Invalid request type for Credit Card Payment.");

        if (string.IsNullOrEmpty(creditCardPaymentRequest.CardNumber))
            return CreateErrorResponse("Card Number is required");

        if (creditCardPaymentRequest.ExpirationMonth is < 1 or > 12)
            return CreateErrorResponse("Expiration Month is out of range");

        if (creditCardPaymentRequest.ExpirationYear < DateTime.Now.Year)
            return CreateErrorResponse("Invalid Expiration Year");

        if (creditCardPaymentRequest.Cvv is < 100 or > 999)
            return CreateErrorResponse("Invalid CVV");

        if (creditCardPaymentRequest.Amount < 0)
            return CreateErrorResponse("Invalid Amount");

        logger.LogInformation(
            "Paid with Credit Card Payment - CardNumber {CardNumber}, Expiration: {ExpirationMonth}/{ExpirationYear}" +
            ", CVV: {Cvv}, Amount: {Amount} ",
            creditCardPaymentRequest.CardNumber,
            creditCardPaymentRequest.ExpirationMonth,
            creditCardPaymentRequest.ExpirationYear,
            creditCardPaymentRequest.Cvv,
            creditCardPaymentRequest.Amount
        );

        var response = new PaymentResponse(
            ConfirmationCode: Guid.NewGuid(),
            Message: "Paid with Credit Card Payment",
            PaymentMethod: PaymentMethod,
            PaidAt: DateTime.UtcNow);

        return Task.FromResult(response);
    }

    private static Task<PaymentResponse> CreateErrorResponse(string errorMessage)
    {
        var response = new PaymentResponse(
            ConfirmationCode: null,
            Message: errorMessage,
            PaymentMethod: null,
            PaidAt: null);

        return Task.FromResult(response);
    }
}