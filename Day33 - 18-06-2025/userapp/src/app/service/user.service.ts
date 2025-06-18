import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { catchError, map, Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})

export class UserService {
  private baseUrl = 'https://dummyjson.com/users';
  private http = inject(HttpClient);

  getUsers(): Observable<any> {
    return this.http.get<any>(this.baseUrl).pipe(
      map((response) => response.users),
      catchError((error) => {
        console.error('Error fetching users:', error);
        return throwError(() => new Error('Failed to fetch users'));
      })
    );
  }
}
