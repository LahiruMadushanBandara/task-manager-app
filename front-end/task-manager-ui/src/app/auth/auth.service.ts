import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { StoredCredentials } from '../models/auth.model';
import { ApiResponse } from '../models/task.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly SESSION_KEY = 'tm_credentials';

  /** Single reactive source of truth for auth state, hydrated once from storage. */
  private readonly credentials = signal<StoredCredentials | null>(this.readStored());

  readonly isLoggedIn = computed(() => this.credentials() !== null);
  readonly username = computed(() => this.credentials()?.username ?? '');

  verify(username: string, password: string): Observable<ApiResponse<unknown>> {
    const encoded = btoa(`${username}:${password}`);
    const headers = new HttpHeaders({ Authorization: `Basic ${encoded}` });
    return this.http.post<ApiResponse<unknown>>(`${environment.apiUrl}/auth/verify`, {}, { headers }).pipe(
      tap(() => {
        const creds: StoredCredentials = { username, encoded };
        sessionStorage.setItem(this.SESSION_KEY, JSON.stringify(creds));
        this.credentials.set(creds);
      })
    );
  }

  getCredentials(): StoredCredentials | null {
    return this.credentials();
  }

  logout(): void {
    if (this.credentials() === null) return;
    sessionStorage.removeItem(this.SESSION_KEY);
    this.credentials.set(null);
    this.router.navigate(['/login']);
  }

  private readStored(): StoredCredentials | null {
    const raw = sessionStorage.getItem(this.SESSION_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as StoredCredentials;
    } catch {
      sessionStorage.removeItem(this.SESSION_KEY);
      return null;
    }
  }
}
