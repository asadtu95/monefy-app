import { Injectable, computed, inject } from '@angular/core';

import { Auth } from './auth';
import { MONTH_NAMES } from '../constants/app.constants';

export interface YearMonth {
  year: number;
  month: number;
}

const SHORT_MONTHS = MONTH_NAMES.map((m) => m.slice(0, 3));

/** First and last day of a budgeting month for any start day. */
export function budgetMonthRange(
  year: number,
  month: number,
  startDay: number,
): { start: Date; end: Date } {
  const startMonthIndex = startDay > 15 ? month - 2 : month - 1;
  return {
    start: new Date(year, startMonthIndex, startDay),
    end: new Date(year, startMonthIndex + 1, startDay - 1),
  };
}

/** "25 Oct" */
export function shortDate(d: Date): string {
  return `${d.getDate()} ${SHORT_MONTHS[d.getMonth()]}`;
}

/**
 * Budgeting months, mirroring the API's MonthRange. With the default start day of
 * 1 these are calendar months. With a custom start day (salary on the 25th, say)
 * a month runs from that day to the day before it in the next month, and is named
 * after the calendar month holding most of its days:
 *   start day 1–15  → "November" = 5 Nov – 4 Dec
 *   start day 16–28 → "November" = 25 Oct – 24 Nov
 */
@Injectable({ providedIn: 'root' })
export class Period {
  private readonly auth = inject(Auth);

  readonly startDay = computed(() => this.auth.profile()?.monthStartDay ?? 1);

  /** True when months don't line up with the calendar, so ranges are worth showing. */
  readonly isCustom = computed(() => this.startDay() !== 1);

  /** The budgeting month today falls in. */
  current(): YearMonth {
    return this.containing(new Date());
  }

  containing(date: Date): YearMonth {
    const day = new Date(date.getFullYear(), date.getMonth(), date.getDate());
    const here = { year: day.getFullYear(), month: day.getMonth() + 1 };
    const { start, end } = this.range(here.year, here.month);

    if (day < start) return this.shift(here, -1);
    if (day > end) return this.shift(here, 1);
    return here;
  }

  range(year: number, month: number): { start: Date; end: Date } {
    return budgetMonthRange(year, month, this.startDay());
  }

  shift(ym: YearMonth, delta: number): YearMonth {
    const d = new Date(ym.year, ym.month - 1 + delta, 1);
    return { year: d.getFullYear(), month: d.getMonth() + 1 };
  }

  label(year: number, month: number): string {
    return `${MONTH_NAMES[month - 1]} ${year}`;
  }

  /** "25 Oct – 24 Nov", or null for plain calendar months. */
  rangeLabel(year: number, month: number): string | null {
    if (!this.isCustom()) return null;

    const { start, end } = this.range(year, month);
    return `${shortDate(start)} – ${shortDate(end)}`;
  }
}
