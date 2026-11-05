import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpTestingController } from '@angular/common/http/testing';

import { ExpenseList } from './expense-list';
import { appTestProviders } from '../../../testing/test-providers';
import { environment } from '../../../../environments/environment';

/** Three Food entries and one Transport, so grouping and percentages are checkable. */
const EXPENSES = [
  { id: 'e1', amount: 10000, date: '2026-08-01', note: 'groceries', paymentMethod: 'Card',
    categoryId: 'food', categoryName: 'Food', categoryIcon: 'utensils', categoryColor: '#eda100',
    createdAt: '2026-08-01T00:00:00Z' },
  { id: 'e2', amount: 5000, date: '2026-08-03', note: null, paymentMethod: 'Cash',
    categoryId: 'food', categoryName: 'Food', categoryIcon: 'utensils', categoryColor: '#eda100',
    createdAt: '2026-08-03T00:00:00Z' },
  { id: 'e3', amount: 3000, date: '2026-08-05', note: 'lunch', paymentMethod: 'Cash',
    categoryId: 'food', categoryName: 'Food', categoryIcon: 'utensils', categoryColor: '#eda100',
    createdAt: '2026-08-05T00:00:00Z' },
  { id: 'e4', amount: 2000, date: '2026-08-06', note: 'petrol', paymentMethod: 'Cash',
    categoryId: 'transport', categoryName: 'Transport', categoryIcon: 'bus', categoryColor: '#2a78d6',
    createdAt: '2026-08-06T00:00:00Z' },
];

describe('ExpenseList', () => {
  let fixture: ComponentFixture<ExpenseList>;
  let http: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ExpenseList],
      providers: appTestProviders(),
    }).compileComponents();

    fixture = TestBed.createComponent(ExpenseList);
    http = TestBed.inject(HttpTestingController);

    await fixture.whenStable();

    http
      .expectOne((r) => r.url === `${environment.apiUrl}/expenses`)
      .flush({ success: true, message: null, data: EXPENSES, errors: null });

    await fixture.whenStable();
    fixture.detectChanges();
  });

  afterEach(() => http.verify());

  function root(): HTMLElement {
    return fixture.nativeElement as HTMLElement;
  }

  function text(): string {
    return root().textContent ?? '';
  }

  it('should create', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('collapses the three Food entries into one category row', () => {
    const names = Array.from(root().querySelectorAll('.group__name')).map((n) =>
      n.textContent?.trim(),
    );

    expect(names).toEqual(['Food', 'Transport']);
  });

  it('sums each category and shows its share of the month', () => {
    // Food 18,000 of 20,000 = 90%; Transport 2,000 = 10%.
    expect(text()).toContain('₨18,000');
    expect(text()).toContain('90%');
    expect(text()).toContain('₨2,000');
    expect(text()).toContain('10%');
  });

  it('counts the entries inside each group', () => {
    expect(text()).toContain('3 entries');
    expect(text()).toContain('1 entry');
  });

  it('sizes each percentage bar to its share', () => {
    const fills = Array.from(root().querySelectorAll<HTMLElement>('.group__fill'));

    expect(fills[0].style.width).toBe('90%');
    expect(fills[1].style.width).toBe('10%');
  });

  it('hides individual entries until a group is opened', () => {
    expect(root().querySelectorAll('.txns--nested').length).toBe(0);

    root().querySelector<HTMLButtonElement>('.group__head')!.click();
    fixture.detectChanges();

    expect(root().querySelectorAll('.txns--nested .txn').length).toBe(3);
    expect(text()).toContain('groceries');
  });
});
