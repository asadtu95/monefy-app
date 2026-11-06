import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Session, SupabaseClient, createClient } from '@supabase/supabase-js';

import { environment } from '../../../environments/environment';
import { User } from '../models/user.model';

const NOT_CONFIGURED =
  'Supabase is not configured. Add your project anon key to ' +
  'src/environments/environment.development.ts (Project Settings → API Keys).';

/** Pulls the message out of the API's ApiResponse envelope, whatever shape it is. */
async function readApiError(response: Response): Promise<string> {
  try {
    const body = (await response.json()) as { message?: string; title?: string };
    if (body?.message) return body.message;
    if (body?.title) return body.title;
  } catch {
    // Non-JSON body — fall through.
  }

  return `Could not create your account (HTTP ${response.status}).`;
}

/**
 * Supabase owns identity (sign-up, sign-in, password reset, token refresh);
 * the .NET API owns the profile. This service is the single place that knows
 * about the session, and the only source of the access token.
 */
@Injectable({
  providedIn: 'root',
})
export class Auth {
  private readonly router = inject(Router);

  /**
   * Created on first use rather than at construction: with a missing key the
   * Supabase client throws, and doing that here would take down app bootstrap
   * instead of showing a readable message on the sign-in screen.
   */
  private supabase: SupabaseClient | null = null;

  private readonly session = signal<Session | null>(null);

  /** False until the stored session has been read back at startup. */
  readonly ready = signal(false);

  readonly isConfigured = Boolean(environment.supabase.url && environment.supabase.anonKey);

  readonly isAuthenticated = computed(() => this.session() !== null);

  readonly email = computed(() => this.session()?.user.email ?? null);

  readonly displayName = computed(() => {
    const user = this.session()?.user;
    if (!user) return null;

    const metadata = user.user_metadata as { full_name?: string } | undefined;
    return metadata?.full_name || user.email || null;
  });

  /** Profile from our own API — richer than the token (currency, etc.). */
  readonly profile = signal<User | null>(null);

  constructor() {
    if (!this.isConfigured) {
      this.ready.set(true);
      return;
    }

    void this.restore();

    // Fires on refresh, sign-out in another tab, and password-recovery links.
    this.client.auth.onAuthStateChange((_event, session) => {
      this.session.set(session);
    });
  }

  get accessToken(): string | null {
    return this.session()?.access_token ?? null;
  }

  /**
   * Registration goes through our own API rather than supabase.auth.signUp:
   * the server creates the account already confirmed, so there is no
   * confirmation email and the user can be signed in immediately afterwards.
   */
  async register(email: string, password: string, fullName: string): Promise<void> {
    let response: Response;

    try {
      response = await fetch(`${environment.apiUrl}/auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password, fullName }),
      });
    } catch {
      // fetch rejects (rather than returning a response) when the server is
      // unreachable, so the browser's bare "Failed to fetch" is all we would
      // otherwise surface. Say which address failed and why it usually does.
      throw new Error(
        `Could not reach the API at ${environment.apiUrl}. ` +
          'Start the backend with: dotnet run --launch-profile http',
      );
    }

    if (!response.ok) {
      throw new Error(await readApiError(response));
    }

    // The account exists and is confirmed — sign in to get a session.
    await this.login(email, password);
  }

  async login(email: string, password: string): Promise<void> {
    const { data, error } = await this.client.auth.signInWithPassword({ email, password });

    if (error) {
      throw new Error(error.message);
    }

    this.session.set(data.session);
  }

  async sendPasswordReset(email: string): Promise<void> {
    const { error } = await this.client.auth.resetPasswordForEmail(email, {
      redirectTo: `${window.location.origin}/auth/login`,
    });

    if (error) {
      throw new Error(error.message);
    }
  }

  /** Updates the name shown in the header, which comes from the Supabase session. */
  async updateDisplayName(fullName: string): Promise<void> {
    const { data, error } = await this.client.auth.updateUser({ data: { full_name: fullName } });
    if (error) {
      throw new Error(error.message);
    }

    // updateUser returns the user, not a session; refresh so displayName picks it up.
    const current = this.session();
    if (current && data.user) {
      this.session.set({ ...current, user: data.user });
    }
  }

  /**
   * Re-checks the current password by signing in with it first, so a borrowed
   * unlocked device can't be used to take over the account.
   */
  async changePassword(currentPassword: string, newPassword: string): Promise<void> {
    const email = this.email();
    if (!email) {
      throw new Error('You are not signed in.');
    }

    const check = await this.client.auth.signInWithPassword({ email, password: currentPassword });
    if (check.error) {
      throw new Error('Your current password is incorrect.');
    }

    this.session.set(check.data.session);

    const { error } = await this.client.auth.updateUser({ password: newPassword });
    if (error) {
      throw new Error(error.message);
    }
  }

  async logout(): Promise<void> {
    await this.client.auth.signOut();
    this.session.set(null);
    this.profile.set(null);
    await this.router.navigate(['/auth/login']);
  }

  /** Clears local state without a round trip — used when the API rejects a token. */
  async forceSignOut(): Promise<void> {
    if (this.isConfigured) {
      await this.client.auth.signOut({ scope: 'local' }).catch(() => undefined);
    }

    this.session.set(null);
    this.profile.set(null);
  }

  private get client(): SupabaseClient {
    if (!this.isConfigured) {
      throw new Error(NOT_CONFIGURED);
    }

    this.supabase ??= createClient(environment.supabase.url, environment.supabase.anonKey, {
      auth: {
        persistSession: true,
        autoRefreshToken: true,
        detectSessionInUrl: true,
      },
    });

    return this.supabase;
  }

  private async restore(): Promise<void> {
    try {
      const { data } = await this.client.auth.getSession();
      this.session.set(data.session);
    } finally {
      this.ready.set(true);
    }
  }
}
