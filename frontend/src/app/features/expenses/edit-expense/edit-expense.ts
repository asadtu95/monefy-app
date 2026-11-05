import { Component, OnInit, inject, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { CategoryApi, ExpenseApi } from '../../../core/services/transactions';
import { Category } from '../../../core/models/category.model';
import { PaymentMethod, TransactionType } from '../../../core/models/enums';
import { iconFor } from '../../../core/constants/app.constants';

@Component({
  selector: 'app-edit-expense',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './edit-expense.html',
  styleUrl: './edit-expense.scss',
})
export class EditExpense implements OnInit {
  /** Bound from the :id route param via withComponentInputBinding(). */
  readonly id = input.required<string>();

  private readonly fb = inject(FormBuilder);
  private readonly expenses = inject(ExpenseApi);
  private readonly categories = inject(CategoryApi);
  private readonly router = inject(Router);

  protected readonly options = signal<Category[]>([]);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly paymentMethods = Object.values(PaymentMethod);
  protected readonly icon = iconFor;

  protected readonly form = this.fb.nonNullable.group({
    amount: [null as number | null, [Validators.required, Validators.min(0.01)]],
    date: ['', Validators.required],
    categoryId: ['', Validators.required],
    paymentMethod: [PaymentMethod.Cash, Validators.required],
    note: [''],
  });

  // Signal inputs are populated after construction, so the load waits for ngOnInit.
  ngOnInit(): void {
    this.categories.getAll(TransactionType.Expense).subscribe({
      next: (list) => this.options.set(list),
      error: () => this.error.set('Could not load categories.'),
    });

    this.expenses.getById(this.id()).subscribe({
      next: (e) => {
        this.form.setValue({
          amount: e.amount,
          date: e.date,
          categoryId: e.categoryId,
          paymentMethod: e.paymentMethod,
          note: e.note ?? '',
        });
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load that expense.');
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

    this.expenses
      .update(this.id(), {
        amount: Number(v.amount),
        date: v.date,
        categoryId: v.categoryId,
        paymentMethod: v.paymentMethod,
        note: v.note || null,
      })
      .subscribe({
        next: () => this.router.navigate(['/expenses']),
        error: (err) => {
          this.error.set(err?.error?.message ?? 'Could not save your changes.');
          this.saving.set(false);
        },
      });
  }
}
