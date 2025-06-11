import { Component,OnInit } from '@angular/core';
import { RecipeService } from '../Service/recipe.service';
import { RecipeModel } from '../Models/Reciepe';
import { Recipe } from '../recipe/recipe';

@Component({
  selector: 'app-recipes',
  imports: [Recipe],
  templateUrl: './recipes.html',
  styleUrl: './recipes.css',
})

export class Recipes {
  Recipes: RecipeModel[] | undefined = undefined;
  constructor(private recipeService: RecipeService) {}

  ngOnInit(): void {
    this.recipeService.getAllRecipes().subscribe({
      next: (data: any) => {
        this.Recipes = data.recipes as RecipeModel[];
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('All recipes loaded');
      },
    });
  }
}
