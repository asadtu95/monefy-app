import { Category } from './category.model';
import { BudgetPeriod } from './enums';

export interface Budget {
  id: string;
  amount: number;
  period: BudgetPeriod;
  startDate: string;
  endDate: string;
  categoryId: string;
  category: Category | null;
  /** Computed server-side from expenses inside the period. */
  spentAmount: number;
  remainingAmount: number;
  percentageUsed: number;
}

export interface CreateBudgetRequest {
  amount: number;
  period: BudgetPeriod;
  startDate: string;
  categoryId: string;
}

export type UpdateBudgetRequest = Partial<CreateBudgetRequest>;
