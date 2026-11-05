import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';

import { Auth } from '../services/auth';
import { environment } from '../../../environments/environment';

/** Attaches the Supabase access token to calls aimed at our own API. */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(Auth).accessToken;

  // Never leak the token to third-party hosts.
  if (!token || !req.url.startsWith(environment.apiUrl)) {
    return next(req);
  }

  return next(
    req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    }),
  );
};
