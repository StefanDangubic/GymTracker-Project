import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { tap } from 'rxjs';
import { LoginRequest, RegisterRequest } from '../../shared/models/auth-requests.model';
import { User } from '../../shared/models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly currentUser = signal<User | null>(null);

  readonly user = this.currentUser.asReadonly();
  readonly isAuthenticated = computed(() => this.currentUser() !== null);

  fetchCurrentUser() {
    return this.http
      .get<User>('/api/auth/me')
      .pipe(tap((user) => this.currentUser.set(user)));
  }

  login(credentials: LoginRequest) {
    return this.http
      .post<User>('/api/auth/login', credentials)
      .pipe(tap((user) => this.currentUser.set(user)));
  }

  register(details: RegisterRequest) {
    return this.http
      .post<User>('/api/auth/register', details)
      .pipe(tap((user) => this.currentUser.set(user)));
  }

  logout() {
    return this.http
      .post<void>('/api/auth/logout', {})
      .pipe(tap(() => this.currentUser.set(null)));
  }

  clearUser(): void {
    this.currentUser.set(null);
  }
}
