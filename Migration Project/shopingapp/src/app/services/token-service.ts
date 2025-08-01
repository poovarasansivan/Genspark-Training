import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  unique_name: string;
  nameid: number;
}

@Injectable({ providedIn: 'root' })
export class TokenService {
  getAccessToken(): string | null {
    return localStorage.getItem('Access-Token');
  }

  getRefereshToken(): string | null {
    return localStorage.getItem('Refresh-Token');
  }

  getToken(): string | null {
    return localStorage.getItem('Access-Token');
  }

  setTokens(accessToken: string, refreshToken: string): void {
    localStorage.setItem('Access-Token', accessToken);
    localStorage.setItem('Refresh-Token', refreshToken);
  }

  clearTokens(): void {
    localStorage.removeItem('Access-Token');
    localStorage.removeItem('Refresh-Token');
  }

  decodeToken(): JwtPayload | null {
    const token = this.getToken();
    if (!token) {
      return null;
    }
    try {
      return jwtDecode<JwtPayload>(token);
    } catch (error) {
      console.error('Invalid', error);
      return null;
    }
  }

  getUserName(): string | null {
    const decoded = this.decodeToken();
    return decoded ? decoded.unique_name : null;
  }

  getUserId(): number | null {
    const decoded = this.decodeToken();
    return decoded ? decoded.nameid : null;
  }
}
