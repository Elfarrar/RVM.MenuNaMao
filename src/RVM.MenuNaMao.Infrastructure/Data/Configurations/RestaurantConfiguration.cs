using RVM.MenuNaMao.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RVM.MenuNaMao.Infrastructure.Data.Configurations;

public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.ToTable("restaurants");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).HasMaxLength(200).IsRequired();
        builder.Property(r => r.Slug).HasMaxLength(200).IsRequired();
        builder.HasIndex(r => r.Slug).IsUnique();
        builder.Property(r => r.Address).HasMaxLength(500);
        builder.Property(r => r.Phone).HasMaxLength(20);
        builder.Property(r => r.LogoUrl).HasMaxLength(500);

        builder.HasMany(r => r.Tables).WithOne(t => t.Restaurant).HasForeignKey(t => t.RestaurantId);
        builder.HasMany(r => r.Categories).WithOne(c => c.Restaurant).HasForeignKey(c => c.RestaurantId);
        builder.HasMany(r => r.Orders).WithOne(o => o.Restaurant).HasForeignKey(o => o.RestaurantId);
        builder.HasMany(r => r.StockItems).WithOne(s => s.Restaurant).HasForeignKey(s => s.RestaurantId);
    }
}
