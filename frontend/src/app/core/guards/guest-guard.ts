import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { Auth } from '../services/auth';

/** Keeps signed-in users off the login and register screens. */
export const guestGuard: CanActivateFn = async () => {
  const auth = inject(Auth);
  const router = inject(Router);

  if (!auth.ready()) {
    await new Promise<void>((resolve) => {
      const timer = setInterval(() => {
        if (auth.ready()) {
          clearInterval(timer);
          resolve();
        }
      }, 20);
    });
  }

  return auth.isAuthenticated() ? router.createUrlTree(['/expenses']) : true;
};
