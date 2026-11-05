import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { Auth } from '../services/auth';

/** Blocks the app shell until a Supabase session exists. */
export const authGuard: CanActivateFn = async (_route, state) => {
  const auth = inject(Auth);
  const router = inject(Router);

  await waitUntilReady(auth);

  if (auth.isAuthenticated()) {
    return true;
  }

  // Remember where they were headed so login can send them back.
  return router.createUrlTree(['/auth/login'], {
    queryParams: { redirectTo: state.url },
  });
};

/** Keeps a page refresh from bouncing to login before the stored session loads. */
async function waitUntilReady(auth: Auth): Promise<void> {
  if (auth.ready()) {
    return;
  }

  await new Promise<void>((resolve) => {
    const timer = setInterval(() => {
      if (auth.ready()) {
        clearInterval(timer);
        resolve();
      }
    }, 20);
  });
}
