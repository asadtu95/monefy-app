export interface Income {
  id: string;
  amount: number;
  date: string;
  note: string | null;
  categoryId: string;
  categoryName: string;
  categoryIcon: string;
  categoryColor: string;
  createdAt: string;
}

export interface CreateIncomeRequest {
  amount: number;
  date: string;
  note?: string | null;
  categoryId: string;
}

export type UpdateIncomeRequest = CreateIncomeRequest;
