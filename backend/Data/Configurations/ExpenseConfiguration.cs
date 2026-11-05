using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monefy.Api.Models.Entities;

namespace Monefy.Api.Data.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("expenses");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.Note).HasMaxLength(500);
        builder.Property(e => e.PaymentMethod).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasOne(e => e.User)
            .WithMany(u => u.Expenses)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Deleting a category that still has expenses must fail, not orphan them.
        builder.HasOne(e => e.Category)
            .WithMany(c => c.Expenses)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Covers the dominant query: this user's expenses over a date range.
        builder.HasIndex(e => new { e.UserId, e.Date });
        builder.HasIndex(e => e.CategoryId);
    }
}
