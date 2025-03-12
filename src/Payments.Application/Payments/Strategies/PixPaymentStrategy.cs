using Microsoft.Extensions.Logging;
using Payments.Application.Payments.Abstractions;
using Payments.Application.Payments.Models;

namespace Payments.Application.Payments.Strategies;

public class PixPaymentStrategy(ILogger<PixPaymentStrategy> logger) : IPaymentStrategy
{
    public PaymentMethod PaymentMethod => PaymentMethod.Pix;

    public Task<PaymentResponse> Pay(PaymentRequest request)
    {
        var pixPaymentRequest = request as PixPaymentRequest
                                ?? throw new ArgumentException("Invalid request type for Pix Payment.");

        if (string.IsNullOrEmpty(pixPaymentRequest.Key))
            return CreateErrorResponse("Key is required");
        
        if (pixPaymentRequest.Amount < 0)
            return CreateErrorResponse("Invalid Amount");
        
        logger.LogInformation("Paid with Pix Payment - Key {Key}, Amount: {Amount} ", 
            pixPaymentRequest.Key, 
            pixPaymentRequest.Amount
            );

        var response = new PaymentResponse(
            ConfirmationCode: Guid.NewGuid(),
            Message: "Paid with Pix Payment",
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