import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RecipeCard } from "./components/recipe-card/recipe-card";
import { RecipePage } from "./pages/recipe-page/recipe-page";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'recipe-app';
}
