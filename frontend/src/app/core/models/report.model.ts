export interface CategoryBreakdown {
  categoryId: string;
  categoryName: string;
  icon: string;
  color: string;
  total: number;
  percentage: number;
  transactionCount: number;
}

export interface DailyTotal {
  date: string;
  income: number;
  expense: number;
}

/** Mirrors the backend DashboardDto for one calendar month. */
export interface Dashboard {
  year: number;
  month: number;
  totalIncome: number;
  totalExpense: number;
  remainingBalance: number;
  monthlySavings: number;
  expenseByCategory: CategoryBreakdown[];
  incomeByCategory: CategoryBreakdown[];
  dailyTotals: DailyTotal[];
  transactionCount: number;
  hasData: boolean;
}
