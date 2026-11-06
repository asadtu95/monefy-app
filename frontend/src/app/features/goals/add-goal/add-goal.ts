import { Component, OnInit, computed, inject, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { GoalApi } from '../../../core/services/transactions';
import { GoalStatus } from '../../../core/models/enums';
import { GOAL_ICONS, goalIconFor, toIsoDate } from '../../../core/constants/app.constants';
import { CurrencyFormatPipe } from '../../../shared/pipes/currency-format-pipe';

/** Creates a goal at /goals/add and edits one at /goals/:id/edit. */
@Component({
  selector: 'app-add-goal',
  imports: [ReactiveFormsModule, RouterLink, CurrencyFormatPipe],
  templateUrl: './add-goal.html',
  styleUrl: './add-goal.scss',
})
export class AddGoal implements OnInit {
  /** Bound from the :id route param; absent when creating. */
  readonly id = input<string>();

  private readonly fb = inject(FormBuilder);
  private readonly api = inject(GoalApi);
  private readonly router = inject(Router);

  protected readonly isEdit = computed(() => Boolean(this.id()));
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly status = signal<GoalStatus>(GoalStatus.Active);

  protected readonly iconKeys = Object.keys(GOAL_ICONS);
  protected readonly icon = goalIconFor;
  protected readonly Status = GoalStatus;
  protected readonly minDate = toIsoDate();

  protected readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(150)]],
    icon: ['target'],
    targetAmount: [null as number | null, [Validators.required, Validators.min(1)]],
    targetDate: [this.defaultTargetDate(), Validators.required],
  });

  /** Live preview of the monthly saving the chosen date implies (new goals only). */
  protected readonly preview = signal<{ monthly: number; months: number } | null>(null);

  constructor() {
    this.form.valueChanges.subscribe(() => this.updatePreview());
    this.updatePreview();
  }

  ngOnInit(): void {
    const id = this.id();
    if (!id) return;

    this.loading.set(true);
    this.api.getById(id).subscribe({
      next: (g) => {
        this.form.setValue({
          name: g.name,
          icon: g.icon ?? 'target',
          targetAmount: g.targetAmount,
          targetDate: g.targetDate,
        });
        this.status.set(g.status);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load that goal.');
        this.loading.set(false);
      },
    });
  }

  protected pickIcon(key: string): void {
    this.form.controls.icon.setValue(key);
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.error.set(null);

    const v = this.form.getRawValue();
    const body = {
      name: v.name.trim(),
      icon: v.icon,
      targetAmount: Number(v.targetAmount),
      targetDate: v.targetDate,
    };

    const id = this.id();
    const request = id
      ? this.api.update(id, { ...body, status: this.status() })
      : this.api.create(body);

    request.subscribe({
      next: () => this.router.navigate(['/goals']),
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Could not save that goal.');
        this.saving.set(false);
      },
    });
  }

  protected cancelGoal(): void {
    if (!confirm('Cancel this goal? Its history is kept and you can re-open it later.')) {
      return;
    }

    this.status.set(GoalStatus.Cancelled);
    this.submit();
  }

  private updatePreview(): void {
    const { targetAmount, targetDate } = this.form.getRawValue();
    if (this.isEdit() || !targetAmount || targetAmount < 1 || !targetDate) {
      this.preview.set(null);
      return;
    }

    const now = new Date();
    const [y, m] = targetDate.split('-').map(Number);
    // Same rule as the API: the current month counts as one to save in.
    const months = Math.max(1, (y - now.getFullYear()) * 12 + (m - (now.getMonth() + 1)) + 1);

    this.preview.set({ months, monthly: Math.ceil(targetAmount / months) });
  }

  private defaultTargetDate(): string {
    const d = new Date();
    d.setFullYear(d.getFullYear() + 1);
    return toIsoDate(d);
  }
}
