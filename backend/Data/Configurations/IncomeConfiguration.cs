using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monefy.Api.Models.Entities;

namespace Monefy.Api.Data.Configurations;

public class IncomeConfiguration : IEntityTypeConfiguration<Income>
{
    public void Configure(EntityTypeBuilder<Income> builder)
    {
        builder.ToTable("incomes");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(i => i.Note).HasMaxLength(500);

        builder.HasOne(i => i.User)
            .WithMany(u => u.Incomes)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Category)
            .WithMany(c => c.Incomes)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => new { i.UserId, i.Date });
        builder.HasIndex(i => i.CategoryId);
    }
}
