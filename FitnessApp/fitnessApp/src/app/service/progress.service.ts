import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Form } from '@angular/forms';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ProgressService {
  private baseUrl: string = 'http://localhost:5246/api/v1';
  private accessToken: string | null = localStorage.getItem('token');
  private http = inject(HttpClient);

  getAllProgressWithPagination(progressFilters: any) {
    const params = {
      pageNumber: progressFilters.pageNumber?.toString() ?? '',
      pageSize: progressFilters.pageSize?.toString() ?? '',
    };

    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };

    return this.http
      .get<any>(`${this.baseUrl}/Progress/paginated`, { headers, params })
      .pipe(
        map((response: any) => {
          const values = response?.data?.data?.$values ?? [];
          const count = response?.data?.totalCount ?? values.length;
          return {
            data: values,
            totalCount: count,
          };
        })
      );
  }

  getProgressByCoachId(coachId: string) {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get<any>(`${this.baseUrl}/Progress/coach/${coachId}`, { headers })
      .pipe(
        map((response: any) => {
          const values = response?.data?.$values ?? [];
          return values;
        })
      );
  }

  getProgressByClientId(clientId: string) {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get<any>(`${this.baseUrl}/Progress/client/${clientId}`, { headers })
      .pipe(
        map((response: any) => {
          const values = response?.data?.$values ?? [];
          return values;
        })
      );
  }

  getProgressByUserId(userId: string) {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get<any>(`${this.baseUrl}/Progress/userprogress/${userId}`, { headers })
      .pipe(
        map((response: any) => {
          const values = response?.data?.$values ?? [];
          return values;
        })
      );
  }

  addNewProgress(progress: any) {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.post<any>(`${this.baseUrl}/Progress`, progress, {
      headers,
    });
  }

  addProgressImage(image: File, progressId: string) {
    const formData = new FormData();
    formData.append('File', image);
    formData.append('ProgressId', progressId);

    return this.http.post<any>(`${this.baseUrl}/progressimage/add`, formData, {
      headers: new HttpHeaders({
        Authorization: `Bearer ${this.accessToken}`,
      }),
    });
  }

  updateProgress(progressId: string, progress: any): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.patch<any>(
      `${this.baseUrl}/Progress/${progressId}`,
      progress,
      { headers }
    );
  }
}
