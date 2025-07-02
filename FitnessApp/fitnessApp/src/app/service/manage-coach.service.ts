import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ManageCoach } from '../model/ManageCoach';

@Injectable({ providedIn: 'root' })
export class ManageCoachService {
  private baseUrl: string = 'http://localhost:5246/api';
  private accessToken: string | null = localStorage.getItem('token');

  constructor(private http: HttpClient) {}

  getAllCoaches(): Observable<ManageCoach[]> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get<ManageCoach[]>(`${this.baseUrl}/CoachClientMap/all`, { headers })
      .pipe(
        map((response: any) => {
          return response.data?.$values;
        })
      );
  }

  addCoachMapping(coachId: string, clientId: string): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    const body = {
      coachId: coachId,
      clientId: clientId,
    };
    return this.http
      .post(`${this.baseUrl}/CoachClientMap/map`, body, { headers });
  }

  deleteCoachMapping(coachId: string,clientId: string): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.delete(`${this.baseUrl}/CoachClientMap/${coachId}/${clientId}`, { headers });
  }

  getCoachOnlyMapping(coachId: string): Observable<ManageCoach[]> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get<ManageCoach[]>(`${this.baseUrl}/CoachClientMap/coach/${coachId}`, { headers })
      .pipe(
        map((response: any) => {
          return response.data?.$values;
        })
      );
  }
}
