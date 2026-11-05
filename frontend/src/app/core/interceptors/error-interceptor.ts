import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { Auth } from '../services/auth';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(Auth);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // The token expired or was revoked — drop the session and re-authenticate.
      if (error.status === 401) {
        void auth.forceSignOut().then(() =>
          router.navigate(['/auth/login'], {
            queryParams: { redirectTo: router.url },
          }),
        );
      }

      return throwError(() => error);
    }),
  );
};
