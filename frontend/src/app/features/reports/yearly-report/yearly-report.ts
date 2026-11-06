import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

import { ReportApi } from '../../../core/services/transactions';
import { Period } from '../../../core/services/period';
import { YearlyReport as YearlyReportData } from '../../../core/models/report.model';
import { CurrencyFormatPipe } from '../../../shared/pipes/currency-format-pipe';
import { MONTH_NAMES, iconFor } from '../../../core/constants/app.constants';

interface MonthColumn {
  label: string;
  income: number;
  expense: number;
  incomeHeight: number;
  expenseHeight: number;
}

@Component({
  selector: 'app-yearly-report',
  imports: [RouterLink, RouterLinkActive, CurrencyFormatPipe],
  templateUrl: './yearly-report.html',
  styleUrl: './yearly-report.scss',
})
export class YearlyReport {
  private readonly api = inject(ReportApi);
  private readonly period = inject(Period);

  protected readonly data = signal<YearlyReportData | null>(null);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);

  protected readonly year = signal(this.period.current().year);

  private readonly peak = computed(() => {
    const months = this.data()?.months ?? [];
    return Math.max(1, ...months.map((m) => Math.max(m.income, m.expense)));
  });

  protected readonly columns = computed<MonthColumn[]>(() =>
    (this.data()?.months ?? []).map((m) => ({
      label: MONTH_NAMES[m.month - 1].slice(0, 3),
      income: m.income,
      expense: m.expense,
      // Floor a non-zero value at 2% so a small month still shows a mark.
      incomeHeight: m.income > 0 ? Math.max(2, (m.income / this.peak()) * 100) : 0,
      expenseHeight: m.expense > 0 ? Math.max(2, (m.expense / this.peak()) * 100) : 0,
    })),
  );

  protected readonly axisTicks = computed(() => [this.peak(), this.peak() / 2, 0]);

  protected readonly hasData = computed(
    () => !!this.data() && (this.data()!.totalIncome > 0 || this.data()!.totalExpense > 0),
  );

  protected readonly monthName = (m: number) => MONTH_NAMES[m - 1];
  protected readonly icon = iconFor;

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.api.yearly(this.year()).subscribe({
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

  protected shiftYear(delta: number): void {
    this.year.update((y) => y + delta);
    this.load();
  }
}
