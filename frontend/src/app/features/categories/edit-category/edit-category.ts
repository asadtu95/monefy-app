import { Component, OnInit, inject, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { CategoryApi } from '../../../core/services/transactions';
import { CATEGORY_COLORS, CATEGORY_ICONS, iconFor } from '../../../core/constants/app.constants';

@Component({
  selector: 'app-edit-category',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './edit-category.html',
  styleUrl: './edit-category.scss',
})
export class EditCategory implements OnInit {
  /** Bound from the :id route param via withComponentInputBinding(). */
  readonly id = input.required<string>();

  private readonly fb = inject(FormBuilder);
  private readonly api = inject(CategoryApi);
  private readonly router = inject(Router);

  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);
  /** Shown read-only: changing it would orphan existing transactions. */
  protected readonly type = signal('');

  protected readonly colors = CATEGORY_COLORS;
  protected readonly iconKeys = Object.keys(CATEGORY_ICONS);
  protected readonly icon = iconFor;

  protected readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    icon: ['ellipsis', Validators.required],
    color: [CATEGORY_COLORS[0] as string, Validators.required],
  });

  ngOnInit(): void {
    this.api.getById(this.id()).subscribe({
      next: (c) => {
        if (c.isDefault) {
          this.error.set('Built-in categories cannot be changed.');
          this.loading.set(false);
          return;
        }

        this.form.setValue({ name: c.name, icon: c.icon, color: c.color });
        this.type.set(c.type);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load that category.');
        this.loading.set(false);
      },
    });
  }

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

    const v = this.form.getRawValue();

    this.api.update(this.id(), { ...v, name: v.name.trim() }).subscribe({
      next: () => this.router.navigate(['/categories']),
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Could not save your changes.');
        this.saving.set(false);
      },
    });
  }
}
