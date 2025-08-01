import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ModelService {
  private baseUrl: string = 'http://localhost:5045/api';
  private http = inject(HttpClient);
  private accessToken: string | null = localStorage.getItem('Access-Token');

  addModle(Model: any): Observable<any> {
    const modelData: any = {
      modelName: Model,
    };
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .post<any>(`${this.baseUrl}/Model`, modelData, { headers })
      .pipe(
        map((response: any) => {
          return response.data;
        })
      );
  }

  getModels(): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get<any>(`${this.baseUrl}/Model`, { headers }).pipe(
      map((response: any) => {
        return response.data;
      })
    );
  }

  deleteModel(modelId: number): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .delete<any>(`${this.baseUrl}/Model/${modelId}`, { headers })
      .pipe(
        map((response: any) => {
          return response.data;
        })
      );
  }
}
