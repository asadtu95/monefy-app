import { EnvironmentProviders, Provider } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { Auth } from '../core/services/auth';

/** Stands in for the real Auth service so tests never construct a Supabase client. */
export class AuthStub {
  readonly ready = () => true;
  readonly isAuthenticated = () => true;
  readonly email = () => 'test@monefy.local';
  readonly displayName = () => 'Test User';
  readonly profile = Object.assign(() => null, { set: () => undefined });
  readonly accessToken = null;

  register = async () => undefined;
  login = async () => undefined;
  sendPasswordReset = async () => undefined;
  logout = async () => undefined;
  forceSignOut = async () => undefined;
}

/**
 * Providers every routed, API-backed component needs under test: a router (for
 * RouterLink/ActivatedRoute), a mock HTTP backend so no request escapes, and a
 * stubbed Auth (the real one needs a Supabase URL and key).
 */
export function appTestProviders(): (Provider | EnvironmentProviders)[] {
  return [
    provideRouter([]),
    provideHttpClient(),
    provideHttpClientTesting(),
    { provide: Auth, useClass: AuthStub },
  ];
}
