using Payments.Api.Extensions;
using Payments.Api.Models;
using Payments.Application.Payments;
using Payments.Application.Payments.Abstractions;
using Payments.Application.Payments.Strategies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.ConfigureJsonOptions();
builder.Services.AddScoped<IPaymentStrategy, CreditCardPaymentStrategy>();
builder.Services.AddScoped<IPaymentStrategy, BoletoPaymentStrategy>();
builder.Services.AddScoped<IPaymentStrategy, PixPaymentStrategy>();
builder.Services.AddScoped<IPaymentStrategyResolver, PaymentStrategyResolver>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.MapPost("/payments", async (PayOrderRequest request, IPaymentService paymentService) =>
{
    var paymentRequest = request.ToPaymentRequest();
    var response = await paymentService.ProcessPayment(paymentRequest, request.PaymentMethod);
    
    return response.ConfirmationCode is null ? 
        Results.BadRequest(response.Message) : 
        Results.Ok(response);
});

app.UseHttpsRedirection();
app.Run();
