import { HttpClient, HttpParams } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { map, Observable } from "rxjs";


@Injectable({ providedIn: 'root' })
export class WorkOutLogService{
    private baseUrl: string = 'http://localhost:5246/api/v1';
    private accessToken: string | null = localStorage.getItem('token');
    private http = inject(HttpClient);

    getAllWorkOutLogsWithPagination(LogFilter: any) {

        const params = new HttpParams({
           fromObject: {
                pageNumber: LogFilter.pageNumber?.toString() ?? '',
                pageSize: LogFilter.pageSize?.toString() ?? '',
            },
        });

        const headers={
            Authorization: `Bearer ${this.accessToken}`,
            'Content-Type': 'application/json',
        }
        return this.http.get<any>(`${this.baseUrl}/WorkOutLog/paginated`, { headers, params }).pipe(
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


    getWorkOutByCoachId(coachId: string) {
        const headers = {
            Authorization: `Bearer ${this.accessToken}`,
            'Content-Type': 'application/json',
        };
        return this.http.get<any>(`${this.baseUrl}/WorkOutLog/coach/${coachId}`, { headers }).pipe(
            map((response: any) => {    
                const values = response?.data?.$values ?? [];
                return values
            })
        );
    }

    getWorkOutByClientId(clientId: string) {
        const headers = {
            Authorization: `Bearer ${this.accessToken}`,
            'Content-Type': 'application/json',
        };
        return this.http.get<any>(`${this.baseUrl}/WorkOutLog/user/${clientId}`, { headers }).pipe(
            map((response: any) => {
                const values = response?.data?.$values ?? [];
                return values;
            })
        );
    }

    addNewWorkOutLog(logs: any) {
        const headers = {
            Authorization: `Bearer ${this.accessToken}`,
            'Content-Type': 'application/json',
        };
        return this.http.post<any>(`${this.baseUrl}/WorkOutLog`, logs, { headers });
    }

    updateWorkOutLog(logid: any,logs:any):Observable<any> {
        const headers = {
            Authorization: `Bearer ${this.accessToken}`,
            'Content-Type': 'application/json',
        };
        return this.http.patch<any>(`${this.baseUrl}/WorkOutLog/${logid}`, logs, { headers });
    }
}