import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

type ParamValue = string | number | boolean | null | undefined;

/**
 * Thin wrapper over HttpClient that unwraps the backend's ApiResponse envelope,
 * so features deal in domain objects rather than `response.data` everywhere.
 */
@Injectable({
  providedIn: 'root',
})
export class Api {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiUrl;

  get<T>(path: string, params?: Record<string, ParamValue>): Observable<T> {
    return this.http
      .get<ApiResponse<T>>(`${this.base}/${path}`, { params: this.toParams(params) })
      .pipe(map((r) => r.data));
  }

  post<T>(path: string, body: unknown): Observable<T> {
    return this.http.post<ApiResponse<T>>(`${this.base}/${path}`, body).pipe(map((r) => r.data));
  }

  put<T>(path: string, body: unknown): Observable<T> {
    return this.http.put<ApiResponse<T>>(`${this.base}/${path}`, body).pipe(map((r) => r.data));
  }

  delete<T>(path: string): Observable<T> {
    return this.http.delete<ApiResponse<T>>(`${this.base}/${path}`).pipe(map((r) => r.data));
  }

  private toParams(params?: Record<string, ParamValue>): HttpParams {
    let httpParams = new HttpParams();

    for (const [key, value] of Object.entries(params ?? {})) {
      if (value !== null && value !== undefined && value !== '') {
        httpParams = httpParams.set(key, String(value));
      }
    }

    return httpParams;
  }
}
