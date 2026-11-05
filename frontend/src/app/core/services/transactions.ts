import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { Api } from './api';
import { Category, CreateCategoryRequest } from '../models/category.model';
import { CreateExpenseRequest, Expense, UpdateExpenseRequest } from '../models/expense.model';
import { CreateIncomeRequest, Income, UpdateIncomeRequest } from '../models/income.model';
import { Dashboard } from '../models/report.model';
import { TransactionType } from '../models/enums';

@Injectable({ providedIn: 'root' })
export class CategoryApi {
  private readonly api = inject(Api);

  getAll(type?: TransactionType): Observable<Category[]> {
    return this.api.get<Category[]>('categories', { type });
  }

  create(body: CreateCategoryRequest): Observable<Category> {
    return this.api.post<Category>('categories', body);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`categories/${id}`);
  }
}

@Injectable({ providedIn: 'root' })
export class ExpenseApi {
  private readonly api = inject(Api);

  getMonth(year: number, month: number): Observable<Expense[]> {
    return this.api.get<Expense[]>('expenses', { year, month });
  }

  getById(id: string): Observable<Expense> {
    return this.api.get<Expense>(`expenses/${id}`);
  }

  create(body: CreateExpenseRequest): Observable<Expense> {
    return this.api.post<Expense>('expenses', body);
  }

  update(id: string, body: UpdateExpenseRequest): Observable<Expense> {
    return this.api.put<Expense>(`expenses/${id}`, body);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`expenses/${id}`);
  }
}

@Injectable({ providedIn: 'root' })
export class IncomeApi {
  private readonly api = inject(Api);

  getMonth(year: number, month: number): Observable<Income[]> {
    return this.api.get<Income[]>('incomes', { year, month });
  }

  getById(id: string): Observable<Income> {
    return this.api.get<Income>(`incomes/${id}`);
  }

  create(body: CreateIncomeRequest): Observable<Income> {
    return this.api.post<Income>('incomes', body);
  }

  update(id: string, body: UpdateIncomeRequest): Observable<Income> {
    return this.api.put<Income>(`incomes/${id}`, body);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`incomes/${id}`);
  }
}

@Injectable({ providedIn: 'root' })
export class DashboardApi {
  private readonly api = inject(Api);

  get(year: number, month: number): Observable<Dashboard> {
    return this.api.get<Dashboard>('dashboard', { year, month });
  }
}
