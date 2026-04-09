using RVM.MenuNaMao.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RVM.MenuNaMao.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();

        builder.HasMany(c => c.MenuItems).WithOne(m => m.Category).HasForeignKey(m => m.CategoryId);
    }
}
