import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { PlanQueryParams } from '../model/WorkOutPlanParms';
import { Observable } from 'rxjs/internal/Observable';
import { map } from 'rxjs';
import { WorkOutPlanModel } from '../model/WorkOutPlan';

@Injectable({ providedIn: 'root' })

export class WorkOutPlanService {
  private baseUrl: string = 'http://localhost:5246/api/v1';
  private accessToken: string | null = localStorage.getItem('token');
  private http = inject(HttpClient);

  getWorkOutPlansByPagination(
    workoutPlanParms: PlanQueryParams
  ): Observable<any> {
    const params = {
      searchTerm: workoutPlanParms.search || '',
      sortBy: workoutPlanParms.sortBy || '',
      sortOrder: workoutPlanParms.sortOrder || 'asc',
      pageNumber: workoutPlanParms.pageNumber || 1,
      pageSize: workoutPlanParms.pageSize || 10,
    };

    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get(`${this.baseUrl}/WorkOutPlan/paginated`, { headers, params })
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

  getTotalCountOfClients(): Observable<number> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get(`${this.baseUrl}/WorkOutPlan/grouped`, { headers })
      .pipe(
        map((response: any) => {
          return response?.data?.$values.length ?? 0;
        })
      );
  }

  AddNewWorkOutPlan(plan: WorkOutPlanModel): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.post(`${this.baseUrl}/WorkOutPlan`, plan, { headers });
  }

  getWorkOutPlans():Observable<any[]> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get<any[]>(`${this.baseUrl}/WorkOutPlan`, { headers }).pipe(
      map((response: any) => {
        return response?.data?.$values ?? [];
      })
    );
  }

  updateWorkOutPlan(id:string, plan : any): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };

    console.log('Updating plan with ID:', id, 'and data:', plan);
    return this.http.put<any>(`${this.baseUrl}/UserPlan/${id}`, plan, { headers });
  }

    getGroupedWorkOutPlans(id: any): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get<any[]>(`${this.baseUrl}/WorkOutPlan/grouped/${id}`, { headers })
      .pipe(
        map((response: any) => {
          return response.data?.$values ?? [];
        })
      );    
  }

  getWorkOutPlanByCoachId(id: string): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get<any[]>(`${this.baseUrl}/WorkOutPlan/grouppedcoach/${id}`, { headers })
      .pipe(
        map((response: any) => {
          return response?.data?.$values ?? [];
        })
      );
  }

  getWorkOutPlanByUserId(id: string): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get<any[]>(`${this.baseUrl}/WorkOutPlan/userenrolledplan/${id}`, { headers })
      .pipe(
        map((response: any) => {
          return response?.data?.$values ?? [];
        })
      );
  }

}
