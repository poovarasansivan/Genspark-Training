import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class NewsService {
  private baseUrl: string = 'http://localhost:5045/api';
  private http = inject(HttpClient);
  private accessToken: string | null = localStorage.getItem('Access-Token');

  getNews(): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get<any>(`${this.baseUrl}/News`, { headers }).pipe(
      map((response: any) => {
        return response.data;
      })
    );
  }

  getNewsById(id: number): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get<any>(`${this.baseUrl}/News/${id}`, { headers }).pipe(
      map((response: any) => {
        return response.data;
      })
    );
  }

  addNews(news: any): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
    };
    return this.http.post<any>(`${this.baseUrl}/News`, news, { headers });
  }

  updateNews(id:any,news: any): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
    };
    return this.http
      .patch<any>(`${this.baseUrl}/News/${id}`, news, { headers })
      .pipe(
        map((response: any) => {
          return response.data;
        })
      );
  }

  deleteNews(id: number): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .delete<any>(`${this.baseUrl}/News/${id}`, { headers })
      .pipe(
        map((response: any) => {
          return response.data;
        })
      );
  }

  exportToExcel(): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get(`${this.baseUrl}/News/export-news`, {
      headers,
      responseType: 'blob',
    });
  }

  exportToCSV(): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get(`${this.baseUrl}/News/export-news-csv`, {
      headers,
      responseType: 'blob',
    });
  }
}
