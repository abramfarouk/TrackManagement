import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

interface LoginResponse {
  token: string;
  expiresAtUtc: string;
}

const TOKEN_KEY = 'trackmanagement.jwt';
const USER_KEY = 'trackmanagement.user';
const SESSION_KEYS = [TOKEN_KEY, USER_KEY, 'trackmanagement.session', 'trackmanagement.lastLogin'];

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenSubject = new BehaviorSubject<string | null>(this.readStoredToken());
  readonly isLoggedIn$ = this.tokenSubject.asObservable();

  constructor(private http: HttpClient) {}

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiBaseUrl}/auth/login`, { username, password })
      .pipe(tap((res) => {
        this.setToken(res.token);
        localStorage.setItem(USER_KEY, username.trim());
      }));
  }

  logout(): void {
    SESSION_KEYS.forEach((key) => localStorage.removeItem(key));
    sessionStorage.clear();
    this.tokenSubject.next(null);
  }

  getToken(): string | null {
    return this.tokenSubject.value;
  }

  isLoggedIn(): boolean {
    return !!this.tokenSubject.value;
  }

  private setToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token);
    this.tokenSubject.next(token);
  }

  private readStoredToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }
}
