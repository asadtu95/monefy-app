/** "ok" below 80% used, "warning" from 80%, "over" past 100%. */
export type BudgetStatus = 'ok' | 'warning' | 'over';

/** A standing monthly limit for one expense category, measured against one month. */
export interface Budget {
  id: string;
  amount: number;
  categoryId: string;
  categoryName: string;
  categoryIcon: string;
  categoryColor: string;
  spent: number;
  /** Negative once the limit is exceeded. */
  remaining: number;
  /** Can exceed 100. */
  percentageUsed: number;
  status: BudgetStatus;
}

export interface BudgetOverview {
  year: number;
  month: number;
  startDate: string;
  endDate: string;
  totalBudget: number;
  totalSpent: number;
  totalRemaining: number;
  percentageUsed: number;
  /** Days left in the month including today; 0 for past months. */
  daysLeft: number;
  budgets: Budget[];
}

export interface CreateBudgetRequest {
  categoryId: string;
  amount: number;
}

export interface UpdateBudgetRequest {
  amount: number;
}
