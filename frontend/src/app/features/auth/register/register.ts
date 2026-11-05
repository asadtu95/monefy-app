import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { Auth } from '../../../core/services/auth';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrls: ['../auth.scss', './register.scss'],
})
export class Register {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(Auth);
  private readonly router = inject(Router);

  protected readonly busy = signal(false);
  protected readonly error = signal<string | null>(null);

  // --- Email-confirmation flow (commented out on request) -------------------
  // Restore this together with the matching block in register.html if you turn
  // "Confirm email" back on in Supabase (Authentication -> Sign In / Providers).
  // protected readonly checkInbox = signal(false);
  // --------------------------------------------------------------------------

  protected readonly form = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(150)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  protected async submit(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.busy.set(true);
    this.error.set(null);

    const { fullName, email, password } = this.form.getRawValue();

    try {
      await this.auth.register(email.trim(), password, fullName.trim());

      // The API creates the account already confirmed and Auth.register signs
      // in straight afterwards, so a session should always exist by here.
      if (this.auth.isAuthenticated()) {
        await this.router.navigateByUrl('/expenses');
        return;
      }

      // --- Email-confirmation flow (commented out on request) ---------------
      // this.checkInbox.set(true);
      // this.busy.set(false);
      // ----------------------------------------------------------------------

      this.error.set('Account created, but sign-in did not complete. Try signing in.');
      this.busy.set(false);
    } catch (err) {
      this.error.set(err instanceof Error ? err.message : 'Could not create your account.');
      this.busy.set(false);
    }
  }
}
