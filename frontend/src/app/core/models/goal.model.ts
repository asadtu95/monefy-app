import { GoalStatus } from './enums';

export interface GoalContribution {
  id: string;
  /** Negative for a withdrawal. */
  amount: number;
  date: string;
  note: string | null;
}

export interface Goal {
  id: string;
  name: string;
  icon: string | null;
  targetAmount: number;
  currentAmount: number;
  remainingAmount: number;
  percentageComplete: number;
  targetDate: string;
  status: GoalStatus;
  monthsLeft: number;
  /** What to save each month to finish on time. 0 once reached. */
  monthlyNeeded: number;
  isOverdue: boolean;
  createdAt: string;
  /** Newest first. */
  contributions: GoalContribution[];
}

export interface CreateGoalRequest {
  name: string;
  targetAmount: number;
  targetDate: string;
  icon?: string | null;
}

export interface UpdateGoalRequest extends CreateGoalRequest {
  status?: GoalStatus;
}

export interface CreateContributionRequest {
  amount: number;
  date: string;
  note?: string | null;
}
