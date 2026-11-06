import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

import { ReportApi } from '../../../core/services/transactions';
import { Period } from '../../../core/services/period';
import { MonthlyReport as MonthlyReportData } from '../../../core/models/report.model';
import { CurrencyFormatPipe } from '../../../shared/pipes/currency-format-pipe';
import { iconFor } from '../../../core/constants/app.constants';

/** Percentage change, or null when there is nothing to compare against. */
export function percentChange(current: number, previous: number): number | null {
  if (previous === 0) return null;
  return Math.round(((current - previous) / previous) * 1000) / 10;
}

@Component({
  selector: 'app-monthly-report',
  imports: [RouterLink, RouterLinkActive, CurrencyFormatPipe],
  templateUrl: './monthly-report.html',
  styleUrl: './monthly-report.scss',
})
export class MonthlyReport {
  private readonly api = inject(ReportApi);
  private readonly period = inject(Period);

  protected readonly data = signal<MonthlyReportData | null>(null);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);

  private readonly initial = this.period.current();
  protected readonly year = signal(this.initial.year);
  protected readonly month = signal(this.initial.month);

  protected readonly monthLabel = computed(() => this.period.label(this.year(), this.month()));
  protected readonly rangeLabel = computed(() =>
    this.period.rangeLabel(this.year(), this.month()),
  );
  protected readonly previousLabel = computed(() => {
    const p = this.period.shift({ year: this.year(), month: this.month() }, -1);
    return this.period.label(p.year, p.month);
  });

  protected readonly changes = computed(() => {
    const d = this.data();
    if (!d) return null;

    return {
      income: percentChange(d.current.income, d.previous.income),
      expense: percentChange(d.current.expense, d.previous.expense),
      net: d.current.net - d.previous.net,
    };
  });

  protected readonly hasData = computed(() => {
    const d = this.data();
    return !!d && (d.current.income > 0 || d.current.expense > 0 || d.previous.expense > 0);
  });

  protected readonly icon = iconFor;
  protected readonly abs = Math.abs;

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.api.monthly(this.year(), this.month()).subscribe({
      next: (data) => {
        this.data.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load this report.');
        this.loading.set(false);
      },
    });
  }

  protected shiftMonth(delta: number): void {
    const next = this.period.shift({ year: this.year(), month: this.month() }, delta);
    this.year.set(next.year);
    this.month.set(next.month);
    this.load();
  }
}
