import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { ExpenseApi } from '../../../core/services/transactions';
import { Period } from '../../../core/services/period';
import { Expense } from '../../../core/models/expense.model';
import { CurrencyFormatPipe } from '../../../shared/pipes/currency-format-pipe';
import { iconFor } from '../../../core/constants/app.constants';

interface CategoryGroup {
  categoryId: string;
  categoryName: string;
  categoryIcon: string;
  categoryColor: string;
  total: number;
  /** Share of the month's total expense, 0–100. */
  percentage: number;
  items: Expense[];
}

type View = 'grouped' | 'all';

@Component({
  selector: 'app-expense-list',
  imports: [RouterLink, CurrencyFormatPipe],
  templateUrl: './expense-list.html',
  styleUrl: './expense-list.scss',
})
export class ExpenseList {
  private readonly api = inject(ExpenseApi);

  protected readonly items = signal<Expense[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly view = signal<View>('grouped');

  /** Category ids whose transactions are showing. */
  private readonly expanded = signal<ReadonlySet<string>>(new Set());

  private readonly period = inject(Period);
  private readonly initial = this.period.current();
  protected readonly year = signal(this.initial.year);
  protected readonly month = signal(this.initial.month);

  protected readonly monthLabel = computed(() => this.period.label(this.year(), this.month()));

  /** "25 Oct – 24 Nov" when months don't follow the calendar. */
  protected readonly rangeLabel = computed(() =>
    this.period.rangeLabel(this.year(), this.month()),
  );

  protected readonly total = computed(() =>
    this.items().reduce((sum, e) => sum + e.amount, 0),
  );

  /** One row per category, biggest spend first. */
  protected readonly groups = computed<CategoryGroup[]>(() => {
    const total = this.total();
    const byCategory = new Map<string, CategoryGroup>();

    for (const expense of this.items()) {
      let group = byCategory.get(expense.categoryId);

      if (!group) {
        group = {
          categoryId: expense.categoryId,
          categoryName: expense.categoryName,
          categoryIcon: expense.categoryIcon,
          categoryColor: expense.categoryColor,
          total: 0,
          percentage: 0,
          items: [],
        };
        byCategory.set(expense.categoryId, group);
      }

      group.total += expense.amount;
      group.items.push(expense);
    }

    return [...byCategory.values()]
      .map((group) => ({
        ...group,
        percentage: total === 0 ? 0 : Math.round((group.total / total) * 1000) / 10,
      }))
      .sort((a, b) => b.total - a.total);
  });

  protected readonly icon = iconFor;

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.api.getMonth(this.year(), this.month()).subscribe({
      next: (list) => {
        this.items.set(list);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load expenses.');
        this.loading.set(false);
      },
    });
  }

  protected shiftMonth(delta: number): void {
    const next = new Date(this.year(), this.month() - 1 + delta, 1);
    this.year.set(next.getFullYear());
    this.month.set(next.getMonth() + 1);
    this.expanded.set(new Set());
    this.load();
  }

  protected setView(view: View): void {
    this.view.set(view);
  }

  protected isExpanded(categoryId: string): boolean {
    return this.expanded().has(categoryId);
  }

  protected toggleGroup(categoryId: string): void {
    this.expanded.update((current) => {
      const next = new Set(current);
      next.has(categoryId) ? next.delete(categoryId) : next.add(categoryId);
      return next;
    });
  }

  protected remove(item: Expense): void {
    if (!confirm(`Delete this ${item.categoryName} expense?`)) {
      return;
    }

    this.api.delete(item.id).subscribe({
      next: () => this.items.update((list) => list.filter((e) => e.id !== item.id)),
      error: () => this.error.set('Could not delete that expense.'),
    });
  }
}
