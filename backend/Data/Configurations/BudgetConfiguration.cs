using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monefy.Api.Models.Entities;

namespace Monefy.Api.Data.Configurations;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.ToTable("budgets");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(b => b.Period).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasOne(b => b.User)
            .WithMany(u => u.Budgets)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Category)
            .WithMany(c => c.Budgets)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => new { b.UserId, b.CategoryId, b.StartDate });
    }
}
