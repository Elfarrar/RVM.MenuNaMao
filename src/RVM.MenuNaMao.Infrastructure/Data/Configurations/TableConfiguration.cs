using RVM.MenuNaMao.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RVM.MenuNaMao.Infrastructure.Data.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.ToTable("tables");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.QrCodeToken).HasMaxLength(64).IsRequired();
        builder.HasIndex(t => t.QrCodeToken).IsUnique();
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasMany(t => t.Orders).WithOne(o => o.Table).HasForeignKey(o => o.TableId);
    }
}
