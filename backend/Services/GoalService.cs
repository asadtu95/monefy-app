using Microsoft.EntityFrameworkCore;
using Monefy.Api.Data;
using Monefy.Api.DTOs.Goals;
using Monefy.Api.Helpers;
using Monefy.Api.Models.Entities;
using Monefy.Api.Models.Enums;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Services;

/// <summary>
/// Savings goals. <see cref="Goal.CurrentAmount"/> is always the sum of the goal's
/// contributions; it is recomputed on every change rather than edited directly.
/// </summary>
public class GoalService(AppDbContext db, ICurrentUserService currentUser) : IGoalService
{
    public async Task<IReadOnlyList<GoalDto>> GetAllAsync(CancellationToken ct = default)
    {
        var goals = await db.Goals
            .AsNoTracking()
            .Include(g => g.Contributions)
            .Where(g => g.UserId == currentUser.UserId)
            .ToListAsync(ct);

        return goals
            // Active first, then achieved, then cancelled; soonest deadline first within each.
            .OrderBy(g => g.Status)
            .ThenBy(g => g.TargetDate)
            .Select(ToDto)
            .ToList();
    }

    public async Task<GoalDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var goal = await db.Goals
                       .AsNoTracking()
                       .Include(g => g.Contributions)
                       .FirstOrDefaultAsync(g => g.Id == id && g.UserId == currentUser.UserId, ct)
                   ?? throw new NotFoundException($"Goal {id} was not found.");

        return ToDto(goal);
    }

    public async Task<GoalDto> CreateAsync(CreateGoalDto dto, CancellationToken ct = default)
    {
        var goal = new Goal
        {
            UserId = currentUser.UserId,
            Name = dto.Name.Trim(),
            Icon = dto.Icon,
            TargetAmount = dto.TargetAmount,
            TargetDate = dto.TargetDate,
            CurrentAmount = 0,
            Status = GoalStatus.Active
        };

        db.Goals.Add(goal);
        await db.SaveChangesAsync(ct);

        return ToDto(goal);
    }

    public async Task<GoalDto> UpdateAsync(Guid id, UpdateGoalDto dto, CancellationToken ct = default)
    {
        var goal = await FindOwnedAsync(id, ct);

        goal.Name = dto.Name.Trim();
        goal.Icon = dto.Icon;
        goal.TargetAmount = dto.TargetAmount;
        goal.TargetDate = dto.TargetDate;

        if (dto.Status == GoalStatus.Cancelled)
        {
            goal.Status = GoalStatus.Cancelled;
        }
        else if (dto.Status == GoalStatus.Active || goal.Status != GoalStatus.Cancelled)
        {
            // Re-opening, or a target change that moves the goal across the finish line.
            goal.Status = StatusFor(goal);
        }

        await db.SaveChangesAsync(ct);

        return ToDto(goal);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var goal = await FindOwnedAsync(id, ct);

        db.Goals.Remove(goal);
        await db.SaveChangesAsync(ct);
    }

    public async Task<GoalDto> AddContributionAsync(
        Guid goalId, CreateContributionDto dto, CancellationToken ct = default)
    {
        if (dto.Amount == 0)
        {
            throw new ValidationFailedException("Amount cannot be zero.");
        }

        var goal = await FindOwnedAsync(goalId, ct);

        if (goal.Status == GoalStatus.Cancelled)
        {
            throw new ValidationFailedException("This goal is cancelled. Re-open it to add money.");
        }

        if (goal.CurrentAmount + dto.Amount < 0)
        {
            throw new ValidationFailedException(
                $"You can withdraw at most {goal.CurrentAmount:N0} from this goal.");
        }

        var contribution = new GoalContribution
        {
            GoalId = goal.Id,
            Amount = dto.Amount,
            Date = dto.Date,
            Note = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim()
        };

        // Adding through the navigation keeps the loaded collection current for the recompute.
        goal.Contributions.Add(contribution);
        Recalculate(goal);

        await db.SaveChangesAsync(ct);

        return ToDto(goal);
    }

    public async Task<GoalDto> DeleteContributionAsync(
        Guid goalId, Guid contributionId, CancellationToken ct = default)
    {
        var goal = await FindOwnedAsync(goalId, ct);

        var contribution = goal.Contributions.FirstOrDefault(c => c.Id == contributionId)
                           ?? throw new NotFoundException($"Contribution {contributionId} was not found.");

        goal.Contributions.Remove(contribution);
        db.GoalContributions.Remove(contribution);
        Recalculate(goal);

        await db.SaveChangesAsync(ct);

        return ToDto(goal);
    }

    private async Task<Goal> FindOwnedAsync(Guid id, CancellationToken ct) =>
        await db.Goals
            .Include(g => g.Contributions)
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == currentUser.UserId, ct)
        ?? throw new NotFoundException($"Goal {id} was not found.");

    private static void Recalculate(Goal goal)
    {
        goal.CurrentAmount = goal.Contributions.Sum(c => c.Amount);

        if (goal.Status != GoalStatus.Cancelled)
        {
            goal.Status = StatusFor(goal);
        }
    }

    private static GoalStatus StatusFor(Goal goal) =>
        goal.CurrentAmount >= goal.TargetAmount ? GoalStatus.Achieved : GoalStatus.Active;

    private static GoalDto ToDto(Goal goal)
    {
        var today = PeriodService.Today;
        var remaining = Math.Max(0, goal.TargetAmount - goal.CurrentAmount);
        var isOpen = goal.Status == GoalStatus.Active;

        // Count the current month, so a goal due later this month still needs one deposit.
        var monthsLeft = (goal.TargetDate.Year - today.Year) * 12 + goal.TargetDate.Month - today.Month + 1;
        var isOverdue = isOpen && goal.TargetDate < today && remaining > 0;

        return new GoalDto
        {
            Id = goal.Id,
            Name = goal.Name,
            Icon = goal.Icon,
            TargetAmount = goal.TargetAmount,
            CurrentAmount = goal.CurrentAmount,
            RemainingAmount = remaining,
            PercentageComplete = goal.TargetAmount == 0
                ? 0
                : Math.Min(100m, Math.Round(goal.CurrentAmount / goal.TargetAmount * 100m, 1)),
            TargetDate = goal.TargetDate,
            Status = goal.Status,
            MonthsLeft = isOpen ? Math.Max(1, monthsLeft) : 0,
            MonthlyNeeded = isOpen && remaining > 0
                ? Math.Ceiling(remaining / Math.Max(1, monthsLeft))
                : 0,
            IsOverdue = isOverdue,
            CreatedAt = goal.CreatedAt,
            Contributions = goal.Contributions
                .OrderByDescending(c => c.Date)
                .ThenByDescending(c => c.CreatedAt)
                .Select(c => new GoalContributionDto
                {
                    Id = c.Id,
                    Amount = c.Amount,
                    Date = c.Date,
                    Note = c.Note
                })
                .ToList()
        };
    }
}
