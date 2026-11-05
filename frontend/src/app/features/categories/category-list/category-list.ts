import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { CategoryApi } from '../../../core/services/transactions';
import { Category } from '../../../core/models/category.model';
import { TransactionType } from '../../../core/models/enums';
import { iconFor } from '../../../core/constants/app.constants';

@Component({
  selector: 'app-category-list',
  imports: [RouterLink],
  templateUrl: './category-list.html',
  styleUrl: './category-list.scss',
})
export class CategoryList {
  private readonly api = inject(CategoryApi);

  protected readonly all = signal<Category[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);

  protected readonly income = computed(() =>
    this.all().filter((c) => c.type === TransactionType.Income),
  );

  protected readonly expense = computed(() =>
    this.all().filter((c) => c.type === TransactionType.Expense),
  );

  protected readonly icon = iconFor;

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.api.getAll().subscribe({
      next: (list) => {
        this.all.set(list);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load categories.');
        this.loading.set(false);
      },
    });
  }

  protected remove(category: Category): void {
    if (!confirm(`Delete the "${category.name}" category?`)) {
      return;
    }

    this.error.set(null);

    this.api.delete(category.id).subscribe({
      next: () => this.all.update((list) => list.filter((c) => c.id !== category.id)),
      // The API refuses if transactions still reference it — surface that reason.
      error: (err) => this.error.set(err?.error?.message ?? 'Could not delete that category.'),
    });
  }
}
