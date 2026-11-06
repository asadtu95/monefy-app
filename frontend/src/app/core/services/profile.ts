import { Injectable, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { Api } from './api';
import { Auth } from './auth';
import { UpdateProfileRequest, User } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class ProfileApi {
  private readonly api = inject(Api);
  private readonly auth = inject(Auth);

  /** Also provisions the profile row server-side on first call. */
  load(): Observable<User> {
    return this.api.get<User>('auth/me').pipe(tap((user) => this.auth.profile.set(user)));
  }

  update(body: UpdateProfileRequest): Observable<User> {
    return this.api.put<User>('auth/me', body).pipe(tap((user) => this.auth.profile.set(user)));
  }
}
