import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ContactService {
  private baseUrl: string = 'http://localhost:5045/api';
  private http = inject(HttpClient);
  private accessToken: string | null = localStorage.getItem('Access-Token');

  submitContactForm(contactDetails: any): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .post<any>(`${this.baseUrl}/Contact`, contactDetails, { headers })
      .pipe(
        map((response: any) => {
          console.log('Contact form submitted successfully:', response);
          return response;
        })
      );
  }

  getContactDetails(contactId: number): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http
      .get<any>(`${this.baseUrl}/Contact/${contactId}`, { headers })
      .pipe(
        map((response: any) => {
          console.log('Fetched contact details:', response);
          return response.data;
        })
      );
  }

  getAllContacts(): Observable<any> {
    const headers = {
      Authorization: `Bearer ${this.accessToken}`,
      'Content-Type': 'application/json',
    };
    return this.http.get<any>(`${this.baseUrl}/Contact`, { headers }).pipe(
      map((response: any) => {
        return response.data;
      })
    );
  }
}
