using RVM.MenuNaMao.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RVM.MenuNaMao.Infrastructure.Data.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("stock_movements");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Quantity).HasPrecision(10, 3);
        builder.Property(m => m.Reason).HasMaxLength(500);
    }
}
