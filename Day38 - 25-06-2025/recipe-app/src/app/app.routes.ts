import { Routes } from '@angular/router';
import { RecipePage } from './pages/recipe-page/recipe-page';
import { RecipeDetail } from './pages/recipe-detail/recipe-detail';

export const routes: Routes = [
  { path: 'recipes', component: RecipePage },
  { path: 'recipes/:id', component: RecipeDetail },
  { path: '', redirectTo: '/recipes', pathMatch: 'full' },
];
