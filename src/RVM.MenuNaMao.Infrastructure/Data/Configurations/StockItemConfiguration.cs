using RVM.MenuNaMao.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RVM.MenuNaMao.Infrastructure.Data.Configurations;

public class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
{
    public void Configure(EntityTypeBuilder<StockItem> builder)
    {
        builder.ToTable("stock_items");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Unit).HasConversion<string>().HasMaxLength(10);
        builder.Property(s => s.Quantity).HasPrecision(10, 3);
        builder.Property(s => s.MinQuantity).HasPrecision(10, 3);

        builder.HasMany(s => s.Movements).WithOne(m => m.StockItem).HasForeignKey(m => m.StockItemId);
    }
}
