import { Component, Input } from '@angular/core';
import { Recipe } from '../../models/recipe';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-recipe-card',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './recipe-card.html',
  styleUrl: './recipe-card.css',
})
export class RecipeCard {
  @Input() recipe!: Recipe;
  constructor(private route: Router) {}

  navigate(id: number) {
    this.route.navigateByUrl(`/recipes/${id}`);
  }
}
