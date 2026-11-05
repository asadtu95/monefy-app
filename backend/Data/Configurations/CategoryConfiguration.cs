using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monefy.Api.Models.Entities;

namespace Monefy.Api.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Icon).HasMaxLength(50).IsRequired();
        builder.Property(c => c.Color).HasMaxLength(9).IsRequired();
        builder.Property(c => c.Type).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasOne(c => c.User)
            .WithMany(u => u.Categories)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // A user cannot have two categories with the same name and direction.
        builder.HasIndex(c => new { c.UserId, c.Name, c.Type })
            .IsUnique()
            .HasFilter("user_id IS NOT NULL");

        // Postgres treats NULLs as distinct, so the index above cannot police the
        // built-in categories. They need their own filtered index.
        builder.HasIndex(c => new { c.Name, c.Type })
            .IsUnique()
            .HasFilter("user_id IS NULL")
            .HasDatabaseName("ix_categories_name_type_default");
    }
}
