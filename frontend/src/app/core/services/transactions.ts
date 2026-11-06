import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { Api } from './api';
import { BudgetOverview, Budget, CreateBudgetRequest, UpdateBudgetRequest } from '../models/budget.model';
import { Category, CreateCategoryRequest, UpdateCategoryRequest } from '../models/category.model';
import { CreateExpenseRequest, Expense, UpdateExpenseRequest } from '../models/expense.model';
import { CreateIncomeRequest, Income, UpdateIncomeRequest } from '../models/income.model';
import {
  CreateContributionRequest,
  CreateGoalRequest,
  Goal,
  UpdateGoalRequest,
} from '../models/goal.model';
import { Dashboard, MonthlyReport, YearlyReport } from '../models/report.model';
import { TransactionType } from '../models/enums';

@Injectable({ providedIn: 'root' })
export class CategoryApi {
  private readonly api = inject(Api);

  getAll(type?: TransactionType): Observable<Category[]> {
    return this.api.get<Category[]>('categories', { type });
  }

  getById(id: string): Observable<Category> {
    return this.api.get<Category>(`categories/${id}`);
  }

  create(body: CreateCategoryRequest): Observable<Category> {
    return this.api.post<Category>('categories', body);
  }

  update(id: string, body: UpdateCategoryRequest): Observable<Category> {
    return this.api.put<Category>(`categories/${id}`, body);
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

@Injectable({ providedIn: 'root' })
export class BudgetApi {
  private readonly api = inject(Api);

  getMonth(year: number, month: number): Observable<BudgetOverview> {
    return this.api.get<BudgetOverview>('budgets', { year, month });
  }

  create(body: CreateBudgetRequest): Observable<Budget> {
    return this.api.post<Budget>('budgets', body);
  }

  update(id: string, body: UpdateBudgetRequest): Observable<Budget> {
    return this.api.put<Budget>(`budgets/${id}`, body);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`budgets/${id}`);
  }
}

@Injectable({ providedIn: 'root' })
export class GoalApi {
  private readonly api = inject(Api);

  getAll(): Observable<Goal[]> {
    return this.api.get<Goal[]>('goals');
  }

  getById(id: string): Observable<Goal> {
    return this.api.get<Goal>(`goals/${id}`);
  }

  create(body: CreateGoalRequest): Observable<Goal> {
    return this.api.post<Goal>('goals', body);
  }

  update(id: string, body: UpdateGoalRequest): Observable<Goal> {
    return this.api.put<Goal>(`goals/${id}`, body);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`goals/${id}`);
  }

  /** A negative amount records a withdrawal. Returns the updated goal. */
  contribute(id: string, body: CreateContributionRequest): Observable<Goal> {
    return this.api.post<Goal>(`goals/${id}/contributions`, body);
  }

  deleteContribution(id: string, contributionId: string): Observable<Goal> {
    return this.api.delete<Goal>(`goals/${id}/contributions/${contributionId}`);
  }
}

@Injectable({ providedIn: 'root' })
export class ReportApi {
  private readonly api = inject(Api);

  monthly(year: number, month: number): Observable<MonthlyReport> {
    return this.api.get<MonthlyReport>('reports/monthly', { year, month });
  }

  yearly(year: number): Observable<YearlyReport> {
    return this.api.get<YearlyReport>('reports/yearly', { year });
  }
}
