using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Monefy.Api.Models.Entities;

namespace Monefy.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Income> Incomes => Set<Income>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<GoalContribution> GoalContributions => Set<GoalContribution>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        ApplySnakeCaseColumnNames(modelBuilder);
    }

    public override int SaveChanges()
    {
        StampTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void StampTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }

    /// <summary>
    /// Postgres folds unquoted identifiers to lower case, so PascalCase columns would
    /// otherwise need quoting in every hand-written query or Supabase policy.
    /// </summary>
    private static void ApplySnakeCaseColumnNames(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (tableName is null)
            {
                continue;
            }

            var storeObject = StoreObjectIdentifier.Table(tableName, entity.GetSchema());

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.GetColumnName(storeObject) ?? property.Name));
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(ToSnakeCase(key.GetName()!));
            }

            foreach (var foreignKey in entity.GetForeignKeys())
            {
                foreignKey.SetConstraintName(ToSnakeCase(foreignKey.GetConstraintName()!));
            }

            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()!));
            }
        }
    }

    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var builder = new StringBuilder(value.Length + 8);

        for (var i = 0; i < value.Length; i++)
        {
            var current = value[i];

            if (char.IsUpper(current))
            {
                var previousIsLower = i > 0 && char.IsLower(value[i - 1]);
                var nextIsLower = i + 1 < value.Length && char.IsLower(value[i + 1]);

                if (i > 0 && value[i - 1] != '_' && (previousIsLower || nextIsLower))
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(current));
            }
            else
            {
                builder.Append(current);
            }
        }

        return builder.ToString();
    }
}
