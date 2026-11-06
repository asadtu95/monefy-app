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

/** Income, expense and savings for one budgeting month. */
export interface MonthTotals {
  year: number;
  month: number;
  startDate: string;
  endDate: string;
  income: number;
  expense: number;
  net: number;
  savings: number;
  savingsRate: number;
}

export interface CategoryComparison {
  categoryId: string;
  categoryName: string;
  icon: string;
  color: string;
  current: number;
  previous: number;
  change: number;
  /** Null when there was no spend in the previous month. */
  changePercent: number | null;
}

export interface MonthlyReport {
  current: MonthTotals;
  previous: MonthTotals;
  expenseCategories: CategoryComparison[];
  averageDailyExpense: number;
  largestExpense: {
    amount: number;
    date: string;
    categoryName: string;
    note: string | null;
  } | null;
}

export interface YearlyReport {
  year: number;
  /** Always twelve entries, January first. */
  months: MonthTotals[];
  totalIncome: number;
  totalExpense: number;
  totalNet: number;
  totalSavings: number;
  savingsRate: number;
  averageMonthlyIncome: number;
  averageMonthlyExpense: number;
  topExpenseCategories: CategoryBreakdown[];
}
