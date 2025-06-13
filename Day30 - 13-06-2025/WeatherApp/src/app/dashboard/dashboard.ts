import { Component } from '@angular/core';
import { Card } from "../card/card";
import { WeatherService } from '../services/weather.service';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  imports: [Card,CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})

export class Dashboard {
 weather$: Observable<any>;

  constructor(private weatherService: WeatherService) {
    this.weather$ = this.weatherService.weather$;
  }
}
