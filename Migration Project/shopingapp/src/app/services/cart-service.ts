import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CartService {
  private baseUrl: string = 'http://localhost:5045/api';
  private http = inject(HttpClient);
  private accessToken: string | null = localStorage.getItem('Access-Token');

  addToCart(cartItems: any[]): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    console.log('Adding to cart:', cartItems);
    return this.http
      .post<any>(`${this.baseUrl}/Cart`, cartItems, { headers })
      .pipe(
        map((response: any) => {
          console.log('Cart items added:', response);
          return response;
        })
      );
  }

  getCartItems(): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get<any>(`${this.baseUrl}/Cart`, { headers }).pipe(
      map((response: any) => {
        return response.data;
      })
    );
  }

  removeFromCart(cartId: number): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .delete<any>(`${this.baseUrl}/Cart/${cartId}`, { headers })
      .pipe(
        map((response: any) => {
          return response.data;
        })
      );
  }

  getCartItemsByUserId(userId: number): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get<any>(`${this.baseUrl}/Cart/user/${userId}`, { headers })
      .pipe(
        map((response: any) => {
          return response.data;
        })
      );
  }

  updateCartItem(id:any, cartItem: any): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .put<any>(`${this.baseUrl}/Cart/${id}`, cartItem, { headers })
      .pipe(
        map((response: any) => {
          return response.data;
        })
      );
  }
}
