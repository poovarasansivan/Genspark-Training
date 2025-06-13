import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Menu } from "./menu/menu";
import { Search } from "./search/search";
import { Dashboard } from "./dashboard/dashboard";

@Component({
  selector: 'app-root',
  imports: [Dashboard, Menu, Search],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'WeatherApp';
}
