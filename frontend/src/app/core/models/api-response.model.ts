/** Mirrors the backend `DTOs/Common` envelope types. */
export interface ApiResponse<T> {
  success: boolean;
  message: string | null;
  data: T;
  errors: string[] | null;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface PagedRequest {
  pageNumber: number;
  pageSize: number;
  sortBy?: string;
  sortDescending?: boolean;
  search?: string;
}

export interface DateRangeFilter {
  from: string;
  to: string;
}
