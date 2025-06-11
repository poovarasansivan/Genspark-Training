import { Component, inject, Input } from '@angular/core';
import { RecipeModel } from '../Models/Reciepe';
import { RecipeService } from '../Service/recipe.service';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-recipe',
  imports: [],
  templateUrl: './recipe.html',
  styleUrl: './recipe.css',
})

export class Recipe {
  @Input() recipe: RecipeModel | null = null;
  private recipeService = inject(RecipeService);
  constructor() {}
  recipes = toSignal(this.recipeService.getAllRecipes(), { initialValue: [] });
}
