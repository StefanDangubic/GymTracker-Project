import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { tap } from 'rxjs';
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

  clearUser(): void {
    this.currentUser.set(null);
  }
}
