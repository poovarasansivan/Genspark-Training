import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class QuoteService {
  constructor(private http: HttpClient) {}

  getQuotes() {
    return this.http.get<{ quote: string }[]>('/assets/quotes.json');
  }

  getRandomQuote() {
    return this.getQuotes().pipe(
      map((quotes: string | any[]) => {
        const randomIndex = Math.floor(Math.random() * quotes.length);
        return quotes[randomIndex].quote;
      })
    );
  }
}