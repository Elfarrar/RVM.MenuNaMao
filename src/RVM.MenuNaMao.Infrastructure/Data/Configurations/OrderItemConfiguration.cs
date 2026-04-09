using RVM.MenuNaMao.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RVM.MenuNaMao.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.UnitPrice).HasPrecision(10, 2);
        builder.Property(i => i.Notes).HasMaxLength(500);
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(i => i.MenuItem).WithMany().HasForeignKey(i => i.MenuItemId);
    }
}
