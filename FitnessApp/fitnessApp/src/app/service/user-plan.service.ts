import { HttpClient } from '@angular/common/http';
import { inject, Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import PlanSummary from '../model/CoachHome';
import { AllWorkOutPlan } from '../model/AllWorkOutPlan';

@Injectable({ providedIn: 'root' })
export class UserPlanService {
  private baseUrl: string = 'http://localhost:5246/api/v1';
  private accessToken: string | null = localStorage.getItem('token');
  private http = inject(HttpClient);

  getWorkOutPlans(): Observable<PlanSummary[]> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get<PlanSummary[]>(`${this.baseUrl}/WorkOutPlan`, {
      headers,
    });
  }

  getAllWorkOutPlans(): Observable<AllWorkOutPlan[]> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get<AllWorkOutPlan[]>(`${this.baseUrl}/UserPlan`, {
      headers,
    });
  }

  addUserPlan(userId: string, workOutPlanId: string): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };

    const body = { userId, workOutPlanId, isCompleted: 'OnGoing' };
    return this.http.post(`${this.baseUrl}/UserPlan`, body, { headers });
  }
}
