import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, catchError, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class WeatherService {
  private apiKey = 'c1ad185d97af447489a94044251306';
  private apiUrl = 'https://api.weatherapi.com/v1/current.json';

  // Behaviour Subject to hold the current weather data and Observable to expose the weather data to components
  private weatherSubject = new BehaviorSubject<any>(null);
  weather$ = this.weatherSubject.asObservable();
  private historyKey = 'weatherSearchHistory';
  private historySubject = new BehaviorSubject<string[]>(this.getHistory());
  history$ = this.historySubject.asObservable();

  constructor(private http: HttpClient) {}

  // Emits the current weather data for a given city and updates the search history
  fetchWeather(city: string) {
    const url = `${this.apiUrl}?key=${this.apiKey}&q=${encodeURIComponent(city)}`;
    this.http
      .get(url)
      .pipe(
        catchError((err) => {
          this.weatherSubject.next({ error: 'City not found or API error' });
          return of(null); 
        })
      )
      .subscribe((data) => {
        if (data) {
          this.weatherSubject.next(data);
          this.updateHistory(city);
        }
      });
  }

  // Updates the search history in localStorage and emits the new history. only keeps the last 5 unique entries.
  
  private updateHistory(city: string) {
    let history = this.getHistory();
    history = [
      city,
      ...history.filter((c) => c.toLowerCase() !== city.toLowerCase()),
    ].slice(0, 5);

    localStorage.setItem(this.historyKey, JSON.stringify(history));

    this.historySubject.next(history);
  }

  private getHistory(): string[] {
    const stored = localStorage.getItem(this.historyKey);
    return stored ? JSON.parse(stored) : [];
  }
}
