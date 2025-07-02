import { HttpClient } from '@angular/common/http';
import { inject, Inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class WorkoutTaskService {
  private baseUrl: string = 'http://localhost:5246/api/v1';
  private http = inject(HttpClient);
  private accessToken: string | null = localStorage.getItem('token');

  addWorkoutTask(workoutTask: any): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.post<any>(`${this.baseUrl}/UserWorkOutTask`, workoutTask, {
      headers,
    });
  }

  getAllWorkoutTasks(userId: string, planId: string): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get<any>(
        `${this.baseUrl}/UserWorkOutTask/userId/${userId}/planId/${planId}`,
        { headers }
      )
      .pipe(
        map((response: any) => {
          const values = response?.data?.$values ?? [];
          const count = response?.data?.totalCount ?? values.length;
          return {
            data: values,
            totalCount: count,
          };
        })
      );
  }

  updateWorkoutTask(workoutTask: any,id:string): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.put<any>(`${this.baseUrl}/UserWorkOutTask/mark-completed/${id}`, workoutTask, {
      headers,
    });
  }
}
