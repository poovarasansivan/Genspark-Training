import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private baseUrl: string = 'http://localhost:5045/api';
  private http = inject(HttpClient);
  private accessToken: string | null = localStorage.getItem('Access-Token');

  addCategory(category: string): Observable<any> {
    const categoryData = { name: category };
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .post<any>(`${this.baseUrl}/Category`, categoryData, { headers })
      .pipe(
        map((response: any) => {
          return response.data;
        })
      );
  }

  getCategories(): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get<any>(`${this.baseUrl}/Category`, { headers }).pipe(
      map((response: any) => {
        return response.data;
      })
    );
  }

  deleteCategory(categoryId: number): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .delete<any>(`${this.baseUrl}/Category/${categoryId}`, { headers })
      .pipe(
        map((response: any) => {
          return response.data;
        })
      );
  }
  
}
