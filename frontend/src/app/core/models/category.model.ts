import { TransactionType } from './enums';

export interface Category {
  id: string;
  name: string;
  icon: string;
  color: string;
  type: TransactionType;
  isDefault: boolean;
}

export interface CreateCategoryRequest {
  name: string;
  icon: string;
  color: string;
  type: TransactionType;
}

export type UpdateCategoryRequest = Partial<CreateCategoryRequest>;
