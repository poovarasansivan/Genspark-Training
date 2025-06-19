import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Menus } from "./menu/menu";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Menus],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'usermodule';
}
