namespace Orders.Api.Models;

public sealed class Order
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Order()
    {
    }

    public static Order Create(string customerName, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name is required.", nameof(customerName));

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        return new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = customerName.Trim(),
            Amount = amount,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}