import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { Users } from '../models/user-model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private baseUrl: string = 'http://localhost:5045/api';
  private http = inject(HttpClient);
  private accessToken: string | null = localStorage.getItem('Access-Token');

  addUser(user: any): Observable<any> {
    const userData: any = {
      username: user.username,
      password: user.password,
    };
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .post<any>(`${this.baseUrl}/User`, userData, { headers })
      .pipe(
        map((response: any) => {
          return response.data;
        })
      );
  }

  getTickets(): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get<any>(`${this.baseUrl}/Contact`, { headers })
      .pipe(map((response: any) => response.data));
  }
}
