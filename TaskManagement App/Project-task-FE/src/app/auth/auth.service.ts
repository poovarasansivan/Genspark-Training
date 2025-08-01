// src/app/auth/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ApiResponse, LoginRequestDto, LoginResponseDto } from './auth.types';
import { Observable, of, catchError, map, tap } from 'rxjs';


@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = 'http://localhost:5093/api/v1/auth';

  constructor(private http: HttpClient, private router: Router) {}

  login(payload: LoginRequestDto): Observable<any> {
  return this.http.post(`${this.baseUrl}/login`, payload).pipe(
    tap((res: any) => {
      if (res.success) {
        this.setSession(res.data);
        const role = res.data.user.role;
        this.router.navigate(['/dashboard']); // Redirect both roles to a common route
      }
    })
  );
}

  refreshToken(): Observable<boolean> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) return of(false);

    return this.http.post(`${this.baseUrl}/refresh`, { refreshToken }).pipe(
      tap((res: any) => {
        if (res.success) this.setSession(res.data);
      }),
      map(res => res.success),
      catchError(() => of(false))
    );
  }

  logout(): void {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      this.cleanSession();
      return;
    }

    this.http.post<ApiResponse<string>>(`${this.baseUrl}/logout`, { refreshToken }).subscribe({
      next: () => {
        this.cleanSession();
      },
      error: () => {
        // Even if logout fails, still clear client data
        this.cleanSession();
      }
    });
  }

  private cleanSession(): void {
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('user');
    document.cookie = 'refreshToken=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    return !!sessionStorage.getItem('accessToken');
  }

  private setSession(data: LoginResponseDto): void {
  sessionStorage.setItem('accessToken', data.accessToken);
  sessionStorage.setItem('user', JSON.stringify(data.user));

  document.cookie = `refreshToken=${data.refreshToken}; path=/; max-age=${60 * 60 * 24 * 7}`;
}

  getAccessToken(): string | null {
    return sessionStorage.getItem('accessToken');
  }

  getRefreshToken(): string | null {
  const match = document.cookie.match(/(^|;)\\s*refreshToken=([^;]*)/);
  return match ? decodeURIComponent(match[2]) : null;
}

  getUserId(): string {
  const token = this.getAccessToken();
  if (!token) return '';

  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload['nameid'];
  } catch (e) {
    console.error('Failed to parse JWT', e);
    return '';
  }
}


  getUser() {
    const user = sessionStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }
}
