import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { Auth } from '../../../core/services/auth';

@Component({
  selector: 'app-forgot-password',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password.html',
  styleUrls: ['../auth.scss', './forgot-password.scss'],
})
export class ForgotPassword {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(Auth);

  protected readonly busy = signal(false);
  protected readonly sent = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
  });

  protected async submit(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.busy.set(true);
    this.error.set(null);

    try {
      await this.auth.sendPasswordReset(this.form.getRawValue().email.trim());
      this.sent.set(true);
    } catch (err) {
      this.error.set(err instanceof Error ? err.message : 'Could not send the reset link.');
    } finally {
      this.busy.set(false);
    }
  }
}
