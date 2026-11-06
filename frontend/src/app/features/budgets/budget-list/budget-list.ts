import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { BudgetApi } from '../../../core/services/transactions';
import { Period } from '../../../core/services/period';
import { Budget, BudgetOverview } from '../../../core/models/budget.model';
import { CurrencyFormatPipe } from '../../../shared/pipes/currency-format-pipe';
import { iconFor } from '../../../core/constants/app.constants';

@Component({
  selector: 'app-budget-list',
  imports: [RouterLink, CurrencyFormatPipe],
  templateUrl: './budget-list.html',
  styleUrl: './budget-list.scss',
})
export class BudgetList {
  private readonly api = inject(BudgetApi);
  private readonly period = inject(Period);

  protected readonly data = signal<BudgetOverview | null>(null);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);

  /** Id of the budget whose limit is being edited inline. */
  protected readonly editingId = signal<string | null>(null);
  protected readonly editAmount = signal<number | null>(null);
  protected readonly saving = signal(false);

  private readonly initial = this.period.current();
  protected readonly year = signal(this.initial.year);
  protected readonly month = signal(this.initial.month);

  protected readonly monthLabel = computed(() => this.period.label(this.year(), this.month()));
  protected readonly rangeLabel = computed(() =>
    this.period.rangeLabel(this.year(), this.month()),
  );

  /** Only the month in progress gets a "per day" figure. */
  protected readonly perDay = computed(() => {
    const d = this.data();
    if (!d || d.daysLeft === 0 || d.totalRemaining <= 0) return null;

    const isCurrent = this.year() === this.initial.year && this.month() === this.initial.month;
    return isCurrent ? d.totalRemaining / d.daysLeft : null;
  });

  protected readonly overCount = computed(
    () => this.data()?.budgets.filter((b) => b.status === 'over').length ?? 0,
  );

  protected readonly icon = iconFor;

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.api.getMonth(this.year(), this.month()).subscribe({
      next: (data) => {
        this.data.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load your budgets.');
        this.loading.set(false);
      },
    });
  }

  protected shiftMonth(delta: number): void {
    const next = this.period.shift({ year: this.year(), month: this.month() }, delta);
    this.year.set(next.year);
    this.month.set(next.month);
    this.cancelEdit();
    this.load();
  }

  protected startEdit(budget: Budget): void {
    this.editingId.set(budget.id);
    this.editAmount.set(budget.amount);
  }

  protected cancelEdit(): void {
    this.editingId.set(null);
    this.editAmount.set(null);
  }

  protected onAmountInput(event: Event): void {
    const value = (event.target as HTMLInputElement).valueAsNumber;
    this.editAmount.set(Number.isNaN(value) ? null : value);
  }

  protected saveEdit(budget: Budget): void {
    const amount = this.editAmount();
    if (amount === null || amount < 1) {
      this.error.set('A limit must be at least 1.');
      return;
    }

    this.saving.set(true);
    this.error.set(null);

    this.api.update(budget.id, { amount }).subscribe({
      next: () => {
        this.saving.set(false);
        this.cancelEdit();
        // Totals and ordering depend on every row, so refetch rather than patch.
        this.load();
      },
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Could not update that budget.');
        this.saving.set(false);
      },
    });
  }

  protected remove(budget: Budget): void {
    if (!confirm(`Remove the ${budget.categoryName} budget? Your expenses stay as they are.`)) {
      return;
    }

    this.api.delete(budget.id).subscribe({
      next: () => this.load(),
      error: () => this.error.set('Could not remove that budget.'),
    });
  }

  protected barClass(budget: Budget): string {
    return budget.status === 'over' ? 'is-over' : budget.status === 'warning' ? 'is-warning' : '';
  }
}
