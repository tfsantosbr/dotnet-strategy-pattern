using Microsoft.Extensions.Logging;
using Payments.Application.Payments.Abstractions;
using Payments.Application.Payments.Models;

namespace Payments.Application.Payments.Strategies;

public class BoletoPaymentStrategy(ILogger<BoletoPaymentStrategy> logger) : IPaymentStrategy
{
    public Task<PaymentResponse> Pay(PaymentRequest request)
    {
        var boletoPaymentRequest = request as BoletoPaymentRequest
                            ?? throw new ArgumentException("Invalid request type for Boleto Payment.");
        
        if (string.IsNullOrEmpty(boletoPaymentRequest.BarCode))
            return CreateErrorResponse("Barcode is required");
        
        if (boletoPaymentRequest.DueDate < DateTime.UtcNow)
            return CreateErrorResponse("Invalid Due Date");
        
        if (boletoPaymentRequest.Amount < 0)
            return CreateErrorResponse("Invalid Amount");

        logger.LogInformation(
            "Paid with Credit Card Payment - BarCode {BarCode}, DueDate: {DueDate}, Amount: {Amount} ",
            boletoPaymentRequest.BarCode,
            boletoPaymentRequest.DueDate,
            boletoPaymentRequest.Amount
        );

        var response = new PaymentResponse(
            ConfirmationCode: Guid.NewGuid(),
            ConfirmationMessage: "Paid with Boleto Payment",
            PaymentMethod: boletoPaymentRequest.PaymentMethod,
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