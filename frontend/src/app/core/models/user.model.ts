export interface User {
  id: string;
  email: string;
  fullName: string | null;
  avatarUrl: string | null;
  currency: string;
  /** Day a budgeting month begins (1–28). 1 = calendar months. */
  monthStartDay: number;
  createdAt: string;
}

export interface UpdateProfileRequest {
  fullName?: string;
  currency?: string;
  monthStartDay?: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  fullName: string;
}

export interface AuthSession {
  accessToken: string;
  refreshToken: string;
  expiresAt: number;
  user: User;
}
