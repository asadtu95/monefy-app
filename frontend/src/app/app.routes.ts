import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth-guard';
import { guestGuard } from './core/guards/guest-guard';

/**
 * Two zones: /auth/* is public (and bounces signed-in users away), everything
 * else sits behind the shell and requires a Supabase session.
 */
export const routes: Routes = [
  {
    path: 'auth',
    canActivate: [guestGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'login' },
      {
        path: 'login',
        loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
      },
      {
        path: 'register',
        loadComponent: () => import('./features/auth/register/register').then((m) => m.Register),
      },
      {
        path: 'forgot-password',
        loadComponent: () =>
          import('./features/auth/forgot-password/forgot-password').then((m) => m.ForgotPassword),
      },
    ],
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./layout/main-layout/main-layout').then((m) => m.MainLayout),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'expenses' },
      {
        path: 'expenses',
        children: [
          {
            path: '',
            loadComponent: () =>
              import('./features/expenses/expense-list/expense-list').then((m) => m.ExpenseList),
          },
          {
            path: 'add',
            loadComponent: () =>
              import('./features/expenses/add-expense/add-expense').then((m) => m.AddExpense),
          },
          {
            path: ':id/edit',
            loadComponent: () =>
              import('./features/expenses/edit-expense/edit-expense').then((m) => m.EditExpense),
          },
        ],
      },
      {
        path: 'incomes',
        children: [
          {
            path: '',
            loadComponent: () =>
              import('./features/incomes/income-list/income-list').then((m) => m.IncomeList),
          },
          {
            path: 'add',
            loadComponent: () =>
              import('./features/incomes/add-income/add-income').then((m) => m.AddIncome),
          },
          {
            path: ':id/edit',
            loadComponent: () =>
              import('./features/incomes/edit-income/edit-income').then((m) => m.EditIncome),
          },
        ],
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.Dashboard),
      },
      {
        path: 'categories',
        children: [
          {
            path: '',
            loadComponent: () =>
              import('./features/categories/category-list/category-list').then((m) => m.CategoryList),
          },
          {
            path: 'add',
            loadComponent: () =>
              import('./features/categories/add-category/add-category').then((m) => m.AddCategory),
          },
          {
            path: ':id/edit',
            loadComponent: () =>
              import('./features/categories/edit-category/edit-category').then(
                (m) => m.EditCategory,
              ),
          },
        ],
      },
      {
        path: 'budgets',
        children: [
          {
            path: '',
            loadComponent: () =>
              import('./features/budgets/budget-list/budget-list').then((m) => m.BudgetList),
          },
          {
            path: 'add',
            loadComponent: () =>
              import('./features/budgets/add-budget/add-budget').then((m) => m.AddBudget),
          },
        ],
      },
      {
        path: 'goals',
        children: [
          {
            path: '',
            loadComponent: () =>
              import('./features/goals/goals-list/goals-list').then((m) => m.GoalsList),
          },
          {
            path: 'add',
            loadComponent: () => import('./features/goals/add-goal/add-goal').then((m) => m.AddGoal),
          },
          {
            // Same form as add; the :id input switches it to edit mode.
            path: ':id/edit',
            loadComponent: () => import('./features/goals/add-goal/add-goal').then((m) => m.AddGoal),
          },
        ],
      },
      {
        path: 'reports',
        children: [
          { path: '', pathMatch: 'full', redirectTo: 'monthly' },
          {
            path: 'monthly',
            loadComponent: () =>
              import('./features/reports/monthly-report/monthly-report').then(
                (m) => m.MonthlyReport,
              ),
          },
          {
            path: 'yearly',
            loadComponent: () =>
              import('./features/reports/yearly-report/yearly-report').then((m) => m.YearlyReport),
          },
        ],
      },
      {
        path: 'settings',
        loadComponent: () => import('./features/settings/settings').then((m) => m.Settings),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
