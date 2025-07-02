import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { Users } from '../model/Users';
import { UserFilter } from '../model/UserFilter';

@Injectable({ providedIn: 'root' })
export class UserService {
  private baseUrl: string = 'http://localhost:5246/api/v1';
  private http = inject(HttpClient);
  private accessToken: string | null = localStorage.getItem('token');

  getAllUsers(): Observable<Users[]> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get<Users[]>(`${this.baseUrl}/User`, { headers });
  }

  getUserOptions(): Observable<any> {
    const headers = { 
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json', 
    };
    return this.http.get<any>(`${this.baseUrl}/User/`, { headers }).pipe(
      map((response: any) => {
        return response;
      })
    );
  }

  getUsers(): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get<any>(`${this.baseUrl}/User`, { headers }).pipe(
      map((response: any) => {
        return response.data?.$values;        
      })
    );
  }

  getUsersPagination(usersFilter: UserFilter): Observable<any> {
    const params = new HttpParams({
      fromObject: {
        pageNumber: usersFilter.pageNumber?.toString() ?? '',
        pageSize: usersFilter.pageSize?.toString() ?? '',
        isActive:
          usersFilter.isActive != null ? usersFilter.isActive.toString() : '',
        sortBy: usersFilter.sortBy ?? '',
        sortDirection: usersFilter.sortDirection ?? '',
      },
    });

    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };

    return this.http
      .get<any>(`${this.baseUrl}/User/paginated`, { headers, params })
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

  addNewUser(user: Users): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.post<any>(`${this.baseUrl}/User`, user, { headers });
  }

  updateUser(userid: any, user: Partial<Users>): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.patch<any>(`${this.baseUrl}/User/${userid}`, user, {
      headers,
    });
  }

  deleteUser(userid: any): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.delete<any>(`${this.baseUrl}/User/${userid}`, { headers });
  }

  getPlanCount(): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get<any>(`${this.baseUrl}/WorkOutPlan`, { headers });
  }

  getPlanAnalysis(): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get<any>(`${this.baseUrl}/UserPlan`, { headers });
  }

  getLogsAnalysis(): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get<any>(`${this.baseUrl}/WorkOutLog`, { headers });
  }
}
