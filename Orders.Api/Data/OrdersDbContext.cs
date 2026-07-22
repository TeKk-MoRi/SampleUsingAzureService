using Microsoft.EntityFrameworkCore;
using UsingAsure.Models;

namespace UsingAsure.Data;

public sealed class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.CustomerName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.CreatedAtUtc)
                .IsRequired();
        });
    }
}