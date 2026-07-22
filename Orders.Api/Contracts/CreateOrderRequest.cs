namespace Orders.Api.Contracts;

public sealed record CreateOrderRequest(
    string CustomerName,
    decimal Amount
);