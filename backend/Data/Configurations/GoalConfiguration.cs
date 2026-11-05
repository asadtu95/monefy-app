using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monefy.Api.Models.Entities;

namespace Monefy.Api.Data.Configurations;

public class GoalConfiguration : IEntityTypeConfiguration<Goal>
{
    public void Configure(EntityTypeBuilder<Goal> builder)
    {
        builder.ToTable("goals");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name).HasMaxLength(150).IsRequired();
        builder.Property(g => g.TargetAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(g => g.CurrentAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(g => g.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(g => g.Icon).HasMaxLength(50);

        builder.HasOne(g => g.User)
            .WithMany(u => u.Goals)
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(g => new { g.UserId, g.Status });
    }
}

public class GoalContributionConfiguration : IEntityTypeConfiguration<GoalContribution>
{
    public void Configure(EntityTypeBuilder<GoalContribution> builder)
    {
        builder.ToTable("goal_contributions");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.Note).HasMaxLength(500);

        builder.HasOne(c => c.Goal)
            .WithMany(g => g.Contributions)
            .HasForeignKey(c => c.GoalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.GoalId);
    }
}
