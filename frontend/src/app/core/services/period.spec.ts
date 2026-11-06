import { TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';

import { Period, budgetMonthRange } from './period';
import { Auth } from './auth';

const iso = (d: Date) =>
  `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;

describe('budgetMonthRange', () => {
  it('is the calendar month for start day 1', () => {
    const r = budgetMonthRange(2026, 2, 1);
    expect([iso(r.start), iso(r.end)]).toEqual(['2026-02-01', '2026-02-28']);
  });

  it('starts in the named month for early start days', () => {
    const r = budgetMonthRange(2026, 11, 5);
    expect([iso(r.start), iso(r.end)]).toEqual(['2026-11-05', '2026-12-04']);
  });

  it('ends in the named month for late start days', () => {
    const r = budgetMonthRange(2026, 11, 25);
    expect([iso(r.start), iso(r.end)]).toEqual(['2026-10-25', '2026-11-24']);
  });

  it('crosses the year boundary', () => {
    const r = budgetMonthRange(2027, 1, 25);
    expect([iso(r.start), iso(r.end)]).toEqual(['2026-12-25', '2027-01-24']);
  });
});

describe('Period', () => {
  const profile = signal<{ monthStartDay: number } | null>(null);

  function create(startDay: number): Period {
    profile.set({ monthStartDay: startDay });
    TestBed.configureTestingModule({ providers: [{ provide: Auth, useValue: { profile } }] });
    return TestBed.inject(Period);
  }

  it('moves to the next month once payday has passed', () => {
    // Salary on the 25th: 28 Nov already belongs to "December" (25 Nov – 24 Dec).
    expect(create(25).containing(new Date(2026, 10, 28))).toEqual({ year: 2026, month: 12 });
  });

  it('stays in the named month before payday', () => {
    expect(create(25).containing(new Date(2026, 10, 10))).toEqual({ year: 2026, month: 11 });
  });

  it('only labels ranges for custom start days', () => {
    expect(create(1).rangeLabel(2026, 11)).toBeNull();
    TestBed.resetTestingModule();
    expect(create(25).rangeLabel(2026, 11)).toBe('25 Oct – 24 Nov');
  });
});
