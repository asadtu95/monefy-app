import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { CategoryApi } from '../../../core/services/transactions';
import { TransactionType } from '../../../core/models/enums';
import { CATEGORY_COLORS, CATEGORY_ICONS, iconFor } from '../../../core/constants/app.constants';

@Component({
  selector: 'app-add-category',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './add-category.html',
  styleUrl: './add-category.scss',
})
export class AddCategory {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(CategoryApi);
  private readonly router = inject(Router);

  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly colors = CATEGORY_COLORS;
  protected readonly iconKeys = Object.keys(CATEGORY_ICONS);
  protected readonly types = Object.values(TransactionType);
  protected readonly icon = iconFor;

  protected readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    type: [TransactionType.Expense, Validators.required],
    icon: ['ellipsis', Validators.required],
    color: [CATEGORY_COLORS[0] as string, Validators.required],
  });

  protected pickIcon(key: string): void {
    this.form.controls.icon.setValue(key);
  }

  protected pickColor(color: string): void {
    this.form.controls.color.setValue(color);
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.error.set(null);

    this.api.create(this.form.getRawValue()).subscribe({
      next: () => this.router.navigate(['/categories']),
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Could not create that category.');
        this.saving.set(false);
      },
    });
  }
}
