import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';

// Auth endpoints own their 401s already: guards turn them into redirect UrlTrees,
// and the login/register forms show an inline error. Auto-redirecting here too would
// race the guard's own navigation (it calls /api/auth/me on every guarded route) and
// can prevent the initial navigation from ever settling. Only unexpected 401s from
// protected resources (e.g. a session that expired mid-use) should trigger this.
const AUTH_ENDPOINTS = ['/api/auth/me', '/api/auth/login', '/api/auth/register'];

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: unknown) => {
      const isAuthEndpoint = AUTH_ENDPOINTS.some((url) => req.url.includes(url));

      if (error instanceof HttpErrorResponse && error.status === 401 && !isAuthEndpoint) {
        authService.clearUser();
        router.navigateByUrl('/login');
      }

      return throwError(() => error);
    })
  );
};
