import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TokenService } from './token.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl: string = 'http://localhost:5246/api/v1';
  private http = inject(HttpClient);

  constructor(private tokenService: TokenService) {}
  login(email: string, password: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/Authentication/login`, {
      email,
      password,
    });
  }

  verifyEmail(email: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/User/forgot-password`, {
      email,
    });
  }

  resetPassword(token: string, password: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/User/reset-password`, {
      token,
      password,
    });
  }

   refreshToken() {
    const refresh = this.tokenService.getRefreshToken();
    return this.http.post<any>(`${this.baseUrl}/Authentication/refresh`, { refreshToken: refresh });
  }
}
