import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { IncomeApi } from '../../../core/services/transactions';
import { Period } from '../../../core/services/period';
import { Income } from '../../../core/models/income.model';
import { CurrencyFormatPipe } from '../../../shared/pipes/currency-format-pipe';
import { iconFor } from '../../../core/constants/app.constants';

@Component({
  selector: 'app-income-list',
  imports: [RouterLink, CurrencyFormatPipe],
  templateUrl: './income-list.html',
  styleUrl: './income-list.scss',
})
export class IncomeList {
  private readonly api = inject(IncomeApi);

  protected readonly items = signal<Income[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);

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
    this.items().reduce((sum, i) => sum + i.amount, 0),
  );

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
        this.error.set('Could not load income.');
        this.loading.set(false);
      },
    });
  }

  protected shiftMonth(delta: number): void {
    const next = new Date(this.year(), this.month() - 1 + delta, 1);
    this.year.set(next.getFullYear());
    this.month.set(next.getMonth() + 1);
    this.load();
  }

  protected remove(item: Income): void {
    if (!confirm(`Delete this ${item.categoryName} entry?`)) {
      return;
    }

    this.api.delete(item.id).subscribe({
      next: () => this.items.update((list) => list.filter((i) => i.id !== item.id)),
      error: () => this.error.set('Could not delete that entry.'),
    });
  }
}
