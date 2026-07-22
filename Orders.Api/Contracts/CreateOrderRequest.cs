namespace UsingAsure.Contracts;

public sealed record CreateOrderRequest(
    string CustomerName,
    decimal Amount
);