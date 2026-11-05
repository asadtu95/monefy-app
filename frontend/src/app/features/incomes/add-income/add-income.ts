import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { CategoryApi, IncomeApi } from '../../../core/services/transactions';
import { Category } from '../../../core/models/category.model';
import { TransactionType } from '../../../core/models/enums';
import { iconFor } from '../../../core/constants/app.constants';

@Component({
  selector: 'app-add-income',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './add-income.html',
  styleUrl: './add-income.scss',
})
export class AddIncome {
  private readonly fb = inject(FormBuilder);
  private readonly incomes = inject(IncomeApi);
  private readonly categories = inject(CategoryApi);
  private readonly router = inject(Router);

  protected readonly options = signal<Category[]>([]);
  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly icon = iconFor;

  protected readonly form = this.fb.nonNullable.group({
    amount: [null as number | null, [Validators.required, Validators.min(0.01)]],
    date: [this.todayIso(), Validators.required],
    categoryId: ['', Validators.required],
    note: [''],
  });

  constructor() {
    this.categories.getAll(TransactionType.Income).subscribe({
      next: (list) => {
        this.options.set(list);
        if (list.length > 0) {
          this.form.controls.categoryId.setValue(list[0].id);
        }
      },
      error: () => this.error.set('Could not load categories.'),
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

    this.incomes
      .create({
        amount: Number(v.amount),
        date: v.date,
        categoryId: v.categoryId,
        note: v.note || null,
      })
      .subscribe({
        next: () => this.router.navigate(['/incomes']),
        error: (err) => {
          this.error.set(err?.error?.message ?? 'Could not save this income.');
          this.saving.set(false);
        },
      });
  }

  private todayIso(): string {
    return new Date().toISOString().slice(0, 10);
  }
}
