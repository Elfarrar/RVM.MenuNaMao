using RVM.MenuNaMao.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RVM.MenuNaMao.Infrastructure.Data.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("menu_items");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Description).HasMaxLength(1000);
        builder.Property(m => m.Price).HasPrecision(10, 2);
        builder.Property(m => m.ImageUrl).HasMaxLength(500);
    }
}
