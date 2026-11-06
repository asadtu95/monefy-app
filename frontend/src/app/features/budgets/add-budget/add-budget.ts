import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';

import { BudgetApi, CategoryApi } from '../../../core/services/transactions';
import { Period } from '../../../core/services/period';
import { Category } from '../../../core/models/category.model';
import { TransactionType } from '../../../core/models/enums';
import { iconFor } from '../../../core/constants/app.constants';

@Component({
  selector: 'app-add-budget',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './add-budget.html',
  styleUrl: './add-budget.scss',
})
export class AddBudget {
  private readonly fb = inject(FormBuilder);
  private readonly budgets = inject(BudgetApi);
  private readonly categories = inject(CategoryApi);
  private readonly period = inject(Period);
  private readonly router = inject(Router);

  /** Expense categories that don't have a budget yet. */
  protected readonly options = signal<Category[]>([]);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly icon = iconFor;

  protected readonly allTaken = computed(() => !this.loading() && this.options().length === 0);

  protected readonly form = this.fb.nonNullable.group({
    categoryId: ['', Validators.required],
    amount: [null as number | null, [Validators.required, Validators.min(1)]],
  });

  constructor() {
    const { year, month } = this.period.current();

    forkJoin({
      categories: this.categories.getAll(TransactionType.Expense),
      overview: this.budgets.getMonth(year, month),
    }).subscribe({
      next: ({ categories, overview }) => {
        const taken = new Set(overview.budgets.map((b) => b.categoryId));
        this.options.set(categories.filter((c) => !taken.has(c.id)));
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load your categories.');
        this.loading.set(false);
      },
    });
  }

  protected pick(id: string): void {
    this.form.controls.categoryId.setValue(id);
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.error.set(null);

    const v = this.form.getRawValue();

    this.budgets.create({ categoryId: v.categoryId, amount: Number(v.amount) }).subscribe({
      next: () => this.router.navigate(['/budgets']),
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Could not save that budget.');
        this.saving.set(false);
      },
    });
  }
}
