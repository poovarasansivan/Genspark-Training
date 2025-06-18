import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from "./menu/menu";
import { Usercard } from "./usercard/usercard";
import { Form } from "./form/form";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'userapp';
}
