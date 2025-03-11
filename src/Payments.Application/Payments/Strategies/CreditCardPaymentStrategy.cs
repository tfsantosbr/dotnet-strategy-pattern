using Microsoft.Extensions.Logging;
using Payments.Application.Payments.Abstractions;
using Payments.Application.Payments.Models;

namespace Payments.Application.Payments.Strategies;

public class CreditCardPaymentStrategy(ILogger<CreditCardPaymentStrategy> logger)
    : IPaymentStrategy<CreditCardPaymentRequest>
{
    public Task<PaymentResponse> Pay(CreditCardPaymentRequest request)
    {
        if (string.IsNullOrEmpty(request.CardNumber))
            return CreateErrorResponse("Card Number is required");

        if (request.ExpirationMonth is < 1 or > 12)
            return CreateErrorResponse("Expiration Month is out of range");

        if (request.ExpirationYear < DateTime.Now.Year)
            return CreateErrorResponse("Invalid Expiration Year");

        if (request.Cvv is < 100 or > 999)
            return CreateErrorResponse("Invalid CVV");
        
        if (request.Amount < 0)
            return CreateErrorResponse("Invalid Amount");

        logger.LogInformation(
            "Paid with Credit Card Payment - CardNumber {CardNumber}, Expiration: {ExpirationMonth}/{ExpirationYear}" +
            ", CVV: {Cvv}, Amount: {Amount} ",
            request.CardNumber,
            request.ExpirationMonth,
            request.ExpirationYear,
            request.Cvv,
            request.Amount
        );

        var response = new PaymentResponse(
            ConfirmationCode: Guid.NewGuid(),
            ConfirmationMessage: "Paid with Credit Card Payment",
            PaymentMethod: request.PaymentMethod,
            PaidAt: DateTime.UtcNow);

        return Task.FromResult(response);
    }

    private static Task<PaymentResponse> CreateErrorResponse(string errorMessage)
    {
        var response = new PaymentResponse(
            ConfirmationCode: null,
            ConfirmationMessage: errorMessage,
            PaymentMethod: null,
            PaidAt: null);

        return Task.FromResult(response);
    }
}