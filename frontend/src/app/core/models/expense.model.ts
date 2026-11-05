import { PaymentMethod } from './enums';

export interface Expense {
  id: string;
  amount: number;
  date: string;
  note: string | null;
  paymentMethod: PaymentMethod;
  categoryId: string;
  categoryName: string;
  categoryIcon: string;
  categoryColor: string;
  createdAt: string;
}

export interface CreateExpenseRequest {
  amount: number;
  date: string;
  note?: string | null;
  paymentMethod: PaymentMethod;
  categoryId: string;
}

export type UpdateExpenseRequest = CreateExpenseRequest;
