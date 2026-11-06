import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';

import { Auth } from '../../core/services/auth';
import { ProfileApi } from '../../core/services/profile';
import { Period, budgetMonthRange, shortDate } from '../../core/services/period';
import { MONTH_NAMES, SUPPORTED_CURRENCIES } from '../../core/constants/app.constants';

function passwordsMatch(group: AbstractControl): ValidationErrors | null {
  const next = group.get('next')?.value;
  const confirm = group.get('confirm')?.value;
  return next && confirm && next !== confirm ? { mismatch: true } : null;
}

@Component({
  selector: 'app-settings',
  imports: [ReactiveFormsModule],
  templateUrl: './settings.html',
  styleUrl: './settings.scss',
})
export class Settings {
  private readonly fb = inject(FormBuilder);
  private readonly profileApi = inject(ProfileApi);
  private readonly period = inject(Period);

  protected readonly auth = inject(Auth);
  protected readonly currencies = SUPPORTED_CURRENCIES;
  protected readonly startDays = Array.from({ length: 28 }, (_, i) => i + 1);

  // ---------- profile & preferences ----------
  protected readonly profileForm = this.fb.nonNullable.group({
    fullName: [this.auth.displayName() ?? '', [Validators.maxLength(150)]],
    currency: [this.auth.profile()?.currency ?? 'PKR', Validators.required],
    monthStartDay: [this.auth.profile()?.monthStartDay ?? 1, Validators.required],
  });

  protected readonly profileSaving = signal(false);
  protected readonly profileMessage = signal<{ ok: boolean; text: string } | null>(null);

  private readonly startDay = toSignal(this.profileForm.controls.monthStartDay.valueChanges, {
    initialValue: this.profileForm.controls.monthStartDay.value,
  });

  /** Shows what "this month" would cover with the chosen start day. */
  protected readonly startDayPreview = computed(() => {
    const day = Number(this.startDay());
    const { year, month } = this.period.current();
    const { start, end } = budgetMonthRange(year, month, day);
    const name = MONTH_NAMES[month - 1];

    return day === 1
      ? `Months follow the calendar: ${name} = ${shortDate(start)} – ${shortDate(end)}.`
      : `${name} would run ${shortDate(start)} – ${shortDate(end)}.`;
  });

  // ---------- password ----------
  protected readonly passwordForm = this.fb.nonNullable.group(
    {
      current: ['', Validators.required],
      next: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(72)]],
      confirm: ['', Validators.required],
    },
    { validators: passwordsMatch },
  );

  protected readonly passwordSaving = signal(false);
  protected readonly passwordMessage = signal<{ ok: boolean; text: string } | null>(null);

  protected async saveProfile(): Promise<void> {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    this.profileSaving.set(true);
    this.profileMessage.set(null);

    const v = this.profileForm.getRawValue();
    const fullName = v.fullName.trim();

    try {
      // The header reads the name from the sign-in session, the API keeps its own copy.
      if (fullName && fullName !== this.auth.displayName()) {
        await this.auth.updateDisplayName(fullName);
      }

      this.profileApi
        .update({ fullName, currency: v.currency, monthStartDay: Number(v.monthStartDay) })
        .subscribe({
          next: () => {
            this.profileMessage.set({ ok: true, text: 'Settings saved.' });
            this.profileSaving.set(false);
          },
          error: (err) => {
            this.profileMessage.set({
              ok: false,
              text: err?.error?.message ?? 'Could not save your settings.',
            });
            this.profileSaving.set(false);
          },
        });
    } catch (e) {
      this.profileMessage.set({ ok: false, text: (e as Error).message });
      this.profileSaving.set(false);
    }
  }

  protected async changePassword(): Promise<void> {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    this.passwordSaving.set(true);
    this.passwordMessage.set(null);

    const { current, next } = this.passwordForm.getRawValue();

    try {
      await this.auth.changePassword(current, next);
      this.passwordForm.reset();
      this.passwordMessage.set({ ok: true, text: 'Password changed.' });
    } catch (e) {
      this.passwordMessage.set({ ok: false, text: (e as Error).message });
    } finally {
      this.passwordSaving.set(false);
    }
  }
}
