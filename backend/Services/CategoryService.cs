using Microsoft.EntityFrameworkCore;
using Monefy.Api.Data;
using Monefy.Api.DTOs.Categories;
using Monefy.Api.Helpers;
using Monefy.Api.Mappings;
using Monefy.Api.Models.Entities;
using Monefy.Api.Models.Enums;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Services;

public class CategoryService(AppDbContext db, ICurrentUserService currentUser) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(
        TransactionType? type, CancellationToken ct = default)
    {
        var userId = currentUser.UserId;

        var query = db.Categories
            .AsNoTracking()
            .Where(c => c.UserId == null || c.UserId == userId);

        if (type is not null)
        {
            query = query.Where(c => c.Type == type);
        }

        var categories = await query
            .OrderBy(c => c.Type)
            .ThenBy(c => c.Name)
            .ToListAsync(ct);

        return categories.Select(c => c.ToDto()).ToList();
    }

    public async Task<CategoryDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var category = await FindAccessibleAsync(id, ct);
        return category.ToDto();
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default)
    {
        var userId = currentUser.UserId;

        var duplicate = await db.Categories.AnyAsync(
            c => c.UserId == userId && c.Name == dto.Name && c.Type == dto.Type, ct);

        if (duplicate)
        {
            throw new ValidationFailedException(
                $"You already have a {dto.Type.ToString().ToLowerInvariant()} category called '{dto.Name}'.");
        }

        var category = new Category
        {
            UserId = userId,
            Name = dto.Name.Trim(),
            Icon = dto.Icon,
            Color = dto.Color,
            Type = dto.Type,
            IsDefault = false
        };

        db.Categories.Add(category);
        await db.SaveChangesAsync(ct);

        return category.ToDto();
    }

    public async Task<CategoryDto> UpdateAsync(
        Guid id, UpdateCategoryDto dto, CancellationToken ct = default)
    {
        var category = await FindOwnedAsync(id, ct);

        category.Name = dto.Name.Trim();
        category.Icon = dto.Icon;
        category.Color = dto.Color;

        await db.SaveChangesAsync(ct);

        return category.ToDto();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var category = await FindOwnedAsync(id, ct);

        var inUse = await db.Expenses.AnyAsync(e => e.CategoryId == id, ct)
                    || await db.Incomes.AnyAsync(i => i.CategoryId == id, ct);

        if (inUse)
        {
            throw new ValidationFailedException(
                "This category still has transactions. Move or delete them first.");
        }

        db.Categories.Remove(category);
        await db.SaveChangesAsync(ct);
    }

    private async Task<Category> FindAccessibleAsync(Guid id, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        return await db.Categories
                   .FirstOrDefaultAsync(c => c.Id == id && (c.UserId == null || c.UserId == userId), ct)
               ?? throw new NotFoundException($"Category {id} was not found.");
    }

    private async Task<Category> FindOwnedAsync(Guid id, CancellationToken ct)
    {
        var category = await FindAccessibleAsync(id, ct);

        if (category.IsDefault)
        {
            throw new ValidationFailedException("Built-in categories cannot be changed or removed.");
        }

        return category;
    }
}
