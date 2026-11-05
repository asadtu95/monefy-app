import { GoalStatus } from './enums';

export interface Goal {
  id: string;
  name: string;
  targetAmount: number;
  currentAmount: number;
  targetDate: string;
  status: GoalStatus;
  icon: string | null;
  percentageComplete: number;
  createdAt: string;
}

export interface CreateGoalRequest {
  name: string;
  targetAmount: number;
  targetDate: string;
  icon?: string | null;
}

export type UpdateGoalRequest = Partial<CreateGoalRequest>;

export interface ContributeToGoalRequest {
  amount: number;
}
