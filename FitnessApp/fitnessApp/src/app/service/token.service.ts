import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  unique_name: string;
  nameid: string;
  email: string;
  role: string;
  exp: number;
}

@Injectable({ providedIn: 'root' })
export class TokenService {
  getAccessToken(): string | null {
    return localStorage.getItem('token');
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  decodeToken(): JwtPayload | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      return jwtDecode<JwtPayload>(token);
    } catch (error) {
      console.error('Invalid token:', error);
      return null;
    }
  }

  getUsername(): string | null {
    const token = this.decodeToken();
    return this.decodeToken()?.unique_name ?? null;
  }

  getUserId(): string | null {
    return this.decodeToken()?.nameid ?? null;
  }

  getEmail(): string | null {
    return this.decodeToken()?.email ?? null;
  }

  getRole(): string | null {
    return this.decodeToken()?.role ?? null;
  }

  isTokenExpired(): boolean {
    const exp = this.decodeToken()?.exp;
    if (!exp) return true;
    return Date.now() >= exp * 1000;
  }

  saveTokens(access: string, refresh: string): void {
    localStorage.setItem('token', access);
    localStorage.setItem('refreshToken', refresh);
  }

  clearTokens(): void {
    localStorage.removeItem('token');
  }
}
