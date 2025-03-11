using Microsoft.Extensions.Logging;
using Payments.Application.Payments.Abstractions;
using Payments.Application.Payments.Models;

namespace Payments.Application.Payments.Strategies;

public class PixPaymentStrategy(ILogger<PixPaymentStrategy> logger) : IPaymentStrategy<PixPaymentRequest>
{
    public Task<PaymentResponse> Pay(PixPaymentRequest request)
    {
        if (string.IsNullOrEmpty(request.Key))
            return CreateErrorResponse("Key is required");
        
        if (request.Amount < 0)
            return CreateErrorResponse("Invalid Amount");
        
        logger.LogInformation("Paid with Pix Payment - Key {Key}, Amount: {Amount} ", request.Key, request.Amount);

        var response = new PaymentResponse(
            ConfirmationCode: Guid.NewGuid(),
            ConfirmationMessage: "Paid with Pix Payment",
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