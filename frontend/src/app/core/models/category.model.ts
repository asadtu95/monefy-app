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

/** Type cannot change once transactions may reference the category. */
export type UpdateCategoryRequest = Omit<CreateCategoryRequest, 'type'>;
