import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest
} from '@angular/common/http';
import { Observable, catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token = this.authService.getToken();

    if (!token) {
      return next.handle(req.clone({ withCredentials: true }));
    }

    const cloned = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });

    return next.handle(cloned).pipe(
      catchError((error) => {
        if (error.status !== 401 || req.url.includes('/auth/')) {
          return throwError(() => error);
        }

        return this.authService.refresh().pipe(
          switchMap(() => next.handle(req.clone({
            setHeaders: { Authorization: `Bearer ${this.authService.getToken()}` },
            withCredentials: true
          }))),
          catchError((refreshError) => {
            this.authService.clearSession();
            return throwError(() => refreshError);
          })
        );
      })
    );
  }
}
