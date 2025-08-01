import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ColorService {
  private baseUrl: string = 'http://localhost:5045/api';
  private http = inject(HttpClient);
  private accessToken: string | null = localStorage.getItem('Access-Token');

  addColor(color: string): Observable<any> {
    const colorData = { colorName: color };
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .post<any>(`${this.baseUrl}/Color`, colorData, { headers })
      .pipe(
        map((response: any) => {
          return response.data;
        })
      );
  }

  getColors(): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get<any>(`${this.baseUrl}/Color`, { headers }).pipe(
      map((response: any) => {
        return response.data;
      })
    );
  }

  deleteColor(colorId: number): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .delete<any>(`${this.baseUrl}/Color/${colorId}`, { headers })
      .pipe(
        map((response: any) => {
          return response.data;
        })
      );
  }
}
