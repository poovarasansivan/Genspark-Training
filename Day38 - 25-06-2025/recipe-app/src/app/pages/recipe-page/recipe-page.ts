import { Component } from '@angular/core';
import { Recipe } from '../../models/recipe';
import { RecipeService } from '../../services/recipe';
import { CommonModule } from '@angular/common';
import { RecipeCard } from '../../components/recipe-card/recipe-card';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-recipe-page',
  imports: [CommonModule,RecipeCard,RouterModule],
  templateUrl: './recipe-page.html',
  styleUrl: './recipe-page.css'
})
export class RecipePage {
 recipes: Recipe[] = [];
  isLoading = true;
  error = '';

  constructor(private recipeService: RecipeService) {}

  ngOnInit(): void {
    this.recipeService.getRecipes().subscribe({
      next: (data) => {
        this.recipes = data.recipes;
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Failed to load recipes.';
        this.isLoading = false;
      }
    });
  }
}
