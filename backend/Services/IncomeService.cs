using Microsoft.EntityFrameworkCore;
using Monefy.Api.Data;
using Monefy.Api.DTOs.Incomes;
using Monefy.Api.Helpers;
using Monefy.Api.Mappings;
using Monefy.Api.Models.Entities;
using Monefy.Api.Models.Enums;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Services;

public class IncomeService(
    AppDbContext db,
    ICurrentUserService currentUser,
    IPeriodService periods) : IIncomeService
{
    public async Task<IReadOnlyList<IncomeDto>> GetAsync(
        int? year, int? month, CancellationToken ct = default)
    {
        var range = await periods.GetRangeAsync(year, month, ct);
        var userId = currentUser.UserId;

        var incomes = await db.Incomes
            .AsNoTracking()
            .Include(i => i.Category)
            .Where(i => i.UserId == userId && i.Date >= range.Start && i.Date <= range.End)
            .OrderByDescending(i => i.Date)
            .ThenByDescending(i => i.CreatedAt)
            .ToListAsync(ct);

        return incomes.Select(i => i.ToDto()).ToList();
    }

    public async Task<IncomeDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var income = await db.Incomes
                         .AsNoTracking()
                         .Include(i => i.Category)
                         .FirstOrDefaultAsync(i => i.Id == id && i.UserId == currentUser.UserId, ct)
                     ?? throw new NotFoundException($"Income {id} was not found.");

        return income.ToDto();
    }

    public async Task<IncomeDto> CreateAsync(CreateIncomeDto dto, CancellationToken ct = default)
    {
        await EnsureCategoryIsIncomeAsync(dto.CategoryId, ct);

        var income = new Income
        {
            UserId = currentUser.UserId,
            CategoryId = dto.CategoryId,
            Amount = dto.Amount,
            Date = dto.Date,
            Note = dto.Note?.Trim()
        };

        db.Incomes.Add(income);
        await db.SaveChangesAsync(ct);

        return await GetByIdAsync(income.Id, ct);
    }

    public async Task<IncomeDto> UpdateAsync(
        Guid id, UpdateIncomeDto dto, CancellationToken ct = default)
    {
        var income = await db.Incomes
                         .FirstOrDefaultAsync(i => i.Id == id && i.UserId == currentUser.UserId, ct)
                     ?? throw new NotFoundException($"Income {id} was not found.");

        await EnsureCategoryIsIncomeAsync(dto.CategoryId, ct);

        income.CategoryId = dto.CategoryId;
        income.Amount = dto.Amount;
        income.Date = dto.Date;
        income.Note = dto.Note?.Trim();

        await db.SaveChangesAsync(ct);

        return await GetByIdAsync(income.Id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var income = await db.Incomes
                         .FirstOrDefaultAsync(i => i.Id == id && i.UserId == currentUser.UserId, ct)
                     ?? throw new NotFoundException($"Income {id} was not found.");

        db.Incomes.Remove(income);
        await db.SaveChangesAsync(ct);
    }

    private async Task EnsureCategoryIsIncomeAsync(Guid categoryId, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        var category = await db.Categories
                           .AsNoTracking()
                           .FirstOrDefaultAsync(
                               c => c.Id == categoryId && (c.UserId == null || c.UserId == userId), ct)
                       ?? throw new NotFoundException($"Category {categoryId} was not found.");

        if (category.Type != TransactionType.Income)
        {
            throw new ValidationFailedException($"'{category.Name}' is an expense category.");
        }
    }
}
