import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { DashboardApi } from '../../core/services/transactions';
import { Period } from '../../core/services/period';
import { Dashboard as DashboardModel } from '../../core/models/report.model';
import { CurrencyFormatPipe } from '../../shared/pipes/currency-format-pipe';
import { iconFor } from '../../core/constants/app.constants';

interface DayColumn {
  day: number;
  label: string;
  income: number;
  expense: number;
  incomeHeight: number;
  expenseHeight: number;
}

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, CurrencyFormatPipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  private readonly api = inject(DashboardApi);

  protected readonly data = signal<DashboardModel | null>(null);
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

  /** Largest single-day value; the shared scale both series are drawn against. */
  private readonly dayPeak = computed(() => {
    const days = this.data()?.dailyTotals ?? [];
    return Math.max(1, ...days.map((d) => Math.max(d.income, d.expense)));
  });

  protected readonly columns = computed<DayColumn[]>(() =>
    (this.data()?.dailyTotals ?? []).map((d) => {
      const day = Number(d.date.slice(8, 10));
      const peak = this.dayPeak();

      return {
        day,
        label: d.date,
        income: d.income,
        expense: d.expense,
        // Floor a non-zero value at 2% so a small day still renders a visible mark.
        incomeHeight: d.income > 0 ? Math.max(2, (d.income / peak) * 100) : 0,
        expenseHeight: d.expense > 0 ? Math.max(2, (d.expense / peak) * 100) : 0,
      };
    }),
  );

  /** Y-axis ticks at 0 / 50% / 100% of the peak, rounded to something readable. */
  protected readonly axisTicks = computed(() => {
    const peak = this.dayPeak();
    return [peak, peak / 2, 0];
  });

  protected readonly hasChartData = computed(() =>
    this.columns().some((c) => c.income > 0 || c.expense > 0),
  );

  protected readonly icon = iconFor;

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.api.get(this.year(), this.month()).subscribe({
      next: (data) => {
        this.data.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load your dashboard. Is the API running?');
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
}
