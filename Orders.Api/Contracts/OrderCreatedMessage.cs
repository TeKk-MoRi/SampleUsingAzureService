namespace UsingAsure.Contracts;

public sealed record OrderCreatedMessage(
    Guid OrderId,
    string CustomerName,
    decimal Amount,
    DateTime CreatedAtUtc
);