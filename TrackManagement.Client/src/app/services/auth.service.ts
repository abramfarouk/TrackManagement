import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, finalize, firstValueFrom, shareReplay, tap } from 'rxjs';
import { environment } from '../../environments/environment';

interface LoginResponse {
  token: string;
  expiresAtUtc: string;
}

interface RegisteredUser {
  username: string;
  role: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenSubject = new BehaviorSubject<string | null>(null);
  private refreshRequest$: Observable<LoginResponse> | null = null;
  readonly isLoggedIn$ = this.tokenSubject.asObservable();

  constructor(private http: HttpClient) {}

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiBaseUrl}/auth/login`, { username, password }, { withCredentials: true })
      .pipe(tap((res) => {
        this.tokenSubject.next(res.token);
      }));
  }

  register(username: string, password: string, role: string): Observable<RegisteredUser> {
    return this.http
      .post<RegisteredUser>(`${environment.apiBaseUrl}/auth/register`, { username, password, role }, { withCredentials: true });
  }

  logout(): void {
    this.clearSession();
    this.http.post(`${environment.apiBaseUrl}/auth/logout`, {}, { withCredentials: true }).subscribe({
      error: () => undefined
    });
  }

  clearSession(): void {
    this.tokenSubject.next(null);
  }

  getToken(): string | null {
    return this.tokenSubject.value;
  }

  isLoggedIn(): boolean {
    return !!this.tokenSubject.value;
  }

  isAdmin(): boolean {
    return this.getRole() === 'Admin';
  }

  getRole(): string | null {
    const token = this.tokenSubject.value;
    if (!token) {
      return null;
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')));
      return payload.role ?? payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? null;
    } catch {
      return null;
    }
  }

  getUsername(): string | null {
    const token = this.tokenSubject.value;
    if (!token) {
      return null;
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')));
      return payload.unique_name
        ?? payload.name
        ?? payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
        ?? payload.sub
        ?? null;
    } catch {
      return null;
    }
  }

  refresh(): Observable<LoginResponse> {
    if (!this.refreshRequest$) {
      this.refreshRequest$ = this.http
        .post<LoginResponse>(`${environment.apiBaseUrl}/auth/refresh`, {}, { withCredentials: true })
        .pipe(
          tap((res) => this.tokenSubject.next(res.token)),
          finalize(() => this.refreshRequest$ = null),
          shareReplay(1)
        );
    }

    return this.refreshRequest$;
  }

  async restoreSession(): Promise<void> {
    if (this.tokenSubject.value) {
      return;
    }

    try {
      await firstValueFrom(this.refresh());
    } catch {
      this.clearSession();
    }
  }
}
