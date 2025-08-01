import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private baseUrl: string = 'http://localhost:5045/api';
  private http = inject(HttpClient);
  private accessToken: string | null = localStorage.getItem('Access-Token');

  getProducts(filters: any): Observable<any> {
    let params = new HttpParams()
      .set('PageNumber', filters.pageNumber || 1)
      .set('PageSize', filters.pageSize || 10);

    if (filters.category) {
      params = params.set('Category', filters.category);
    }
    if (filters.search) {
      params = params.set('Search', filters.search);
    }
    if (filters.minPrice) {
      params = params.set('MinPrice', filters.minPrice);
    }
    if (filters.maxPrice) {
      params = params.set('MaxPrice', filters.maxPrice);
    }

    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };

    return this.http
      .get<any>(`${this.baseUrl}/Product/pagination`, { headers, params })
      .pipe(
        map((response: any) => {
          const products = response.data ?? [];
          return {
            products,
            totalCount: products.length,
          };
        })
      );
  }

  createProduct(product: any): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
    };
    return this.http.post<any>(`${this.baseUrl}/Product`, product, { headers });
  }

  getProductById(id: string): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
    };
    return this.http.get<any>(`${this.baseUrl}/Product/${id}`, { headers });
  }

  updateProduct(product: any): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
    };
    return this.http.patch<any>(`${this.baseUrl}/Product`, product, {
      headers,
    });
  }

  deleteProduct(id: string): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
    };
    return this.http.delete<any>(`${this.baseUrl}/Product/${id}`, { headers });
  }
}
