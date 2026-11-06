import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { GoalApi } from '../../../core/services/transactions';
import { Goal } from '../../../core/models/goal.model';
import { GoalStatus } from '../../../core/models/enums';
import { CurrencyFormatPipe } from '../../../shared/pipes/currency-format-pipe';
import { MONTH_NAMES, goalIconFor, toIsoDate } from '../../../core/constants/app.constants';

type Panel = 'add' | 'history';

@Component({
  selector: 'app-goals-list',
  imports: [RouterLink, ReactiveFormsModule, CurrencyFormatPipe],
  templateUrl: './goals-list.html',
  styleUrl: './goals-list.scss',
})
export class GoalsList {
  private readonly api = inject(GoalApi);
  private readonly fb = inject(FormBuilder);

  protected readonly goals = signal<Goal[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);

  /** Which goal has which panel open — one at a time keeps the page calm. */
  protected readonly openGoalId = signal<string | null>(null);
  protected readonly openPanel = signal<Panel | null>(null);
  protected readonly saving = signal(false);
  protected readonly panelError = signal<string | null>(null);

  protected readonly Status = GoalStatus;
  protected readonly icon = goalIconFor;

  protected readonly active = computed(() =>
    this.goals().filter((g) => g.status === GoalStatus.Active),
  );

  protected readonly totals = computed(() => {
    const open = this.active();
    return {
      saved: this.goals()
        .filter((g) => g.status !== GoalStatus.Cancelled)
        .reduce((sum, g) => sum + g.currentAmount, 0),
      target: open.reduce((sum, g) => sum + g.targetAmount, 0),
      monthly: open.reduce((sum, g) => sum + g.monthlyNeeded, 0),
      achieved: this.goals().filter((g) => g.status === GoalStatus.Achieved).length,
    };
  });

  protected readonly contribution = this.fb.nonNullable.group({
    direction: ['in' as 'in' | 'out'],
    amount: [null as number | null, [Validators.required, Validators.min(0.01)]],
    date: [toIsoDate(), Validators.required],
    note: [''],
  });

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.api.getAll().subscribe({
      next: (list) => {
        this.goals.set(list);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load your goals.');
        this.loading.set(false);
      },
    });
  }

  protected isOpen(goal: Goal, panel: Panel): boolean {
    return this.openGoalId() === goal.id && this.openPanel() === panel;
  }

  protected toggle(goal: Goal, panel: Panel): void {
    this.panelError.set(null);

    if (this.isOpen(goal, panel)) {
      this.openGoalId.set(null);
      this.openPanel.set(null);
      return;
    }

    if (panel === 'add') {
      this.contribution.reset({ direction: 'in', amount: null, date: toIsoDate(), note: '' });
    }

    this.openGoalId.set(goal.id);
    this.openPanel.set(panel);
  }

  protected saveContribution(goal: Goal): void {
    if (this.contribution.invalid) {
      this.contribution.markAllAsTouched();
      return;
    }

    const v = this.contribution.getRawValue();
    const amount = Number(v.amount) * (v.direction === 'out' ? -1 : 1);

    this.saving.set(true);
    this.panelError.set(null);

    this.api.contribute(goal.id, { amount, date: v.date, note: v.note || null }).subscribe({
      next: (updated) => {
        this.replace(updated);
        this.saving.set(false);
        this.openGoalId.set(null);
        this.openPanel.set(null);
      },
      error: (err) => {
        this.panelError.set(err?.error?.message ?? 'Could not save that.');
        this.saving.set(false);
      },
    });
  }

  protected removeContribution(goal: Goal, contributionId: string): void {
    if (!confirm('Remove this entry? The goal balance will be recalculated.')) {
      return;
    }

    this.api.deleteContribution(goal.id, contributionId).subscribe({
      next: (updated) => this.replace(updated),
      error: () => this.panelError.set('Could not remove that entry.'),
    });
  }

  protected setStatus(goal: Goal, status: GoalStatus): void {
    this.api
      .update(goal.id, {
        name: goal.name,
        icon: goal.icon,
        targetAmount: goal.targetAmount,
        targetDate: goal.targetDate,
        status,
      })
      .subscribe({
        next: () => this.load(),
        error: (err) => this.error.set(err?.error?.message ?? 'Could not update that goal.'),
      });
  }

  protected remove(goal: Goal): void {
    if (!confirm(`Delete "${goal.name}" and its history? This can't be undone.`)) {
      return;
    }

    this.api.delete(goal.id).subscribe({
      next: () => this.goals.update((list) => list.filter((g) => g.id !== goal.id)),
      error: () => this.error.set('Could not delete that goal.'),
    });
  }

  /** "Mar 2027" */
  protected monthYear(iso: string): string {
    const [y, m] = iso.split('-').map(Number);
    return `${MONTH_NAMES[m - 1].slice(0, 3)} ${y}`;
  }

  private replace(updated: Goal): void {
    // Status (and so ordering) may have changed — reload if it did.
    const before = this.goals().find((g) => g.id === updated.id);
    if (before && before.status !== updated.status) {
      this.load();
      return;
    }

    this.goals.update((list) => list.map((g) => (g.id === updated.id ? updated : g)));
  }
}
