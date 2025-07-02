import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UserStatsService {
  private baseUrl: string = 'http://localhost:5246/api/v1';
  private accessToken: string | null = localStorage.getItem('token');
  private http = inject(HttpClient);

  private userId = sessionStorage.getItem('userId');

  getWorkOutStats(userId: any, workoutplanId: any): Observable<any> {
    const url = `${this.baseUrl}/WorkOutLog/user/${userId}/workoutplan/${workoutplanId}`;

    return this.http.get<any>(url, {
      headers: {
        Authorization: `Bearer ${this.accessToken}`,
      },
    });
  }

  getProgressStats(userId: any, workoutplanId: any): Observable<any> {
    const url = `${this.baseUrl}/Progress/user/${userId}/workout/${workoutplanId}`;
    return this.http.get<any>(url, {
      headers: {
        Authorization: `Bearer ${this.accessToken}`,
      },
    });
  }
}
