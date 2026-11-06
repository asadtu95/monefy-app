using Microsoft.EntityFrameworkCore;
using Monefy.Api.Data;
using Monefy.Api.DTOs.Expenses;
using Monefy.Api.Helpers;
using Monefy.Api.Mappings;
using Monefy.Api.Models.Entities;
using Monefy.Api.Models.Enums;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Services;

public class ExpenseService(
    AppDbContext db,
    ICurrentUserService currentUser,
    IPeriodService periods) : IExpenseService
{
    public async Task<IReadOnlyList<ExpenseDto>> GetAsync(
        int? year, int? month, CancellationToken ct = default)
    {
        var range = await periods.GetRangeAsync(year, month, ct);
        var userId = currentUser.UserId;

        var expenses = await db.Expenses
            .AsNoTracking()
            .Include(e => e.Category)
            .Where(e => e.UserId == userId && e.Date >= range.Start && e.Date <= range.End)
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.CreatedAt)
            .ToListAsync(ct);

        return expenses.Select(e => e.ToDto()).ToList();
    }

    public async Task<ExpenseDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var expense = await db.Expenses
                          .AsNoTracking()
                          .Include(e => e.Category)
                          .FirstOrDefaultAsync(e => e.Id == id && e.UserId == currentUser.UserId, ct)
                      ?? throw new NotFoundException($"Expense {id} was not found.");

        return expense.ToDto();
    }

    public async Task<ExpenseDto> CreateAsync(CreateExpenseDto dto, CancellationToken ct = default)
    {
        await EnsureCategoryIsExpenseAsync(dto.CategoryId, ct);

        var expense = new Expense
        {
            UserId = currentUser.UserId,
            CategoryId = dto.CategoryId,
            Amount = dto.Amount,
            Date = dto.Date,
            Note = dto.Note?.Trim(),
            PaymentMethod = dto.PaymentMethod
        };

        db.Expenses.Add(expense);
        await db.SaveChangesAsync(ct);

        return await GetByIdAsync(expense.Id, ct);
    }

    public async Task<ExpenseDto> UpdateAsync(
        Guid id, UpdateExpenseDto dto, CancellationToken ct = default)
    {
        var expense = await db.Expenses
                          .FirstOrDefaultAsync(e => e.Id == id && e.UserId == currentUser.UserId, ct)
                      ?? throw new NotFoundException($"Expense {id} was not found.");

        await EnsureCategoryIsExpenseAsync(dto.CategoryId, ct);

        expense.CategoryId = dto.CategoryId;
        expense.Amount = dto.Amount;
        expense.Date = dto.Date;
        expense.Note = dto.Note?.Trim();
        expense.PaymentMethod = dto.PaymentMethod;

        await db.SaveChangesAsync(ct);

        return await GetByIdAsync(expense.Id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var expense = await db.Expenses
                          .FirstOrDefaultAsync(e => e.Id == id && e.UserId == currentUser.UserId, ct)
                      ?? throw new NotFoundException($"Expense {id} was not found.");

        db.Expenses.Remove(expense);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Stops an income category being attached to an expense.</summary>
    private async Task EnsureCategoryIsExpenseAsync(Guid categoryId, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        var category = await db.Categories
                           .AsNoTracking()
                           .FirstOrDefaultAsync(
                               c => c.Id == categoryId && (c.UserId == null || c.UserId == userId), ct)
                       ?? throw new NotFoundException($"Category {categoryId} was not found.");

        if (category.Type != TransactionType.Expense)
        {
            throw new ValidationFailedException($"'{category.Name}' is an income category.");
        }
    }
}
