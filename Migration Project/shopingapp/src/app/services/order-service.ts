import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private baseUrl: string = 'http://localhost:5045/api';
  private http = inject(HttpClient);
  private accessToken: string | null = localStorage.getItem('Access-Token');

  placeOrder(orderDetails: any): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .post<any>(`${this.baseUrl}/Order`, orderDetails, { headers })
      .pipe(
        map((response: any) => {
          console.log('Order placed successfully:', response);
          return response;
        })
      );
  }

  getOrdersByUserId(userId: number): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get<any>(`${this.baseUrl}/Order/user/${userId}`, { headers })
      .pipe(
        map((response: any) => {
          return response.data;
        })
      );
  }

  getOrderDetails(orderId: number): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get<any>(`${this.baseUrl}/Order/${orderId}`, { headers })
      .pipe(
        map((response: any) => {
          console.log('Fetched order details:', response);
          return response.data;
        })
      );
  }

  getPageniatedOrderDetails(filters: any): Observable<any> {
    let params = new HttpParams()
      .set('PageNumber', filters.pageNumber || 1)
      .set('PageSize', filters.pageSize || 10);
    if (filters.userName) {
      params = params.set('UserName', filters.userName);
    }
    if (filters.status) {
      params = params.set('Status', filters.status);
    }
    if (filters.sortBy) {
      params = params.set('SortBy', filters.sortBy);
    }
    if (filters.sortDirection) {
      params = params.set('SortDirection', filters.sortDirection);
    }

    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };

    return this.http
      .get<any>(`${this.baseUrl}/Order/paginated`, { headers, params })
      .pipe(
        map((response: any) => {
          const orders = response.data ?? [];
          console.log(response);
          return {
            orders,
            totalCount: response.totalCount || 0,
          };
        })
      );
  }
}
