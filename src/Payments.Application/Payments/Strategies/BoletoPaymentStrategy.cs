using Microsoft.Extensions.Logging;
using Payments.Application.Payments.Abstractions;
using Payments.Application.Payments.Models;

namespace Payments.Application.Payments.Strategies;

public class BoletoPaymentStrategy(ILogger<BoletoPaymentStrategy> logger)
    : IPaymentStrategy<BoletoPaymentRequest>
{
    public Task<PaymentResponse> Pay(BoletoPaymentRequest request)
    {
        if (string.IsNullOrEmpty(request.BarCode))
            return CreateErrorResponse("Barcode is required");
        
        if (request.DueDate < DateTime.UtcNow)
            return CreateErrorResponse("Invalid Due Date");
        
        if (request.Amount < 0)
            return CreateErrorResponse("Invalid Amount");

        logger.LogInformation(
            "Paid with Credit Card Payment - BarCode {BarCode}, DueDate: {DueDate}, Amount: {Amount} ",
            request.BarCode,
            request.DueDate,
            request.Amount
        );

        var response = new PaymentResponse(
            ConfirmationCode: Guid.NewGuid(),
            ConfirmationMessage: "Paid with Boleto Payment",
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