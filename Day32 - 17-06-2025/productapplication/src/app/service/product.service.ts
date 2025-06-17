import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private baseUrl = 'https://dummyjson.com/products/search';
  private http = inject(HttpClient);

  getSearchProducts(term: string, skip: number) {
    const params = { q: term, limit: 10, skip };
    return this.http.get<any>(this.baseUrl, { params });
  }
}
