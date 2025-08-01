import { HttpClient } from '@angular/common/http';
import { inject, Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TokenService } from './token-service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl: string = 'http://localhost:5045/api';
  private http = inject(HttpClient);

  constructor(private tokenService:TokenService) {}

  login(username: string, password: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/Auth/login`, {
      username,
      password,
    });
  }

  refreshToken() {
    const refresh = this.tokenService.getRefereshToken();
    return this.http.post<any>(`${this.baseUrl}/Authentication/refresh`, {
      refreshToken: refresh,
    });
  }
}
