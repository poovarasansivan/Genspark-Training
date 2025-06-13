import { Component } from '@angular/core';
import { WeatherService } from '../services/weather.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-search',
  imports: [FormsModule,CommonModule],
  templateUrl: './search.html',
  styleUrl: './search.css',
})
export class Search {
  city: string = '';
  weatherData: any;
  history: string[] =[];

  constructor(private weatherService: WeatherService) {}

  // Subscribe to weather data
  ngOnInit() {
    this.weatherService.history$.subscribe(data => {
      this.history = data;
    });
  }

  searchCity() {
    if (this.city.trim()) {
      this.weatherService.fetchWeather(this.city.trim());
    }
  }

  selectFromHistory(city: string) {
    this.city = city;
    this.searchCity();
  }
}
