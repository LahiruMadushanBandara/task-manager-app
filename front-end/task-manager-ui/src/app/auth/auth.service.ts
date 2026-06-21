import { Injectable, inject } from '@angular/core';
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

  verify(username: string, password: string): Observable<ApiResponse<unknown>> {
    const encoded = btoa(`${username}:${password}`);
    const headers = new HttpHeaders({ Authorization: `Basic ${encoded}` });
    return this.http.post<ApiResponse<unknown>>(`${environment.apiUrl}/auth/verify`, {}, { headers }).pipe(
      tap(() => {
        const creds: StoredCredentials = { username, encoded };
        sessionStorage.setItem(this.SESSION_KEY, JSON.stringify(creds));
      })
    );
  }

  getCredentials(): StoredCredentials | null {
    const raw = sessionStorage.getItem(this.SESSION_KEY);
    return raw ? (JSON.parse(raw) as StoredCredentials) : null;
  }

  isLoggedIn(): boolean {
    return this.getCredentials() !== null;
  }

  logout(): void {
    if (!this.isLoggedIn()) return;
    sessionStorage.removeItem(this.SESSION_KEY);
    this.router.navigate(['/login']);
  }
}
