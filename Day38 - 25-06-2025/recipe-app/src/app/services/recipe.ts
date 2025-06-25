import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Recipe } from '../models/recipe';

@Injectable({
  providedIn: 'root',
})
export class RecipeService {
  private apiUrl = 'https://dummyjson.com/recipes';

  constructor(private http: HttpClient) {}

  getRecipes(): Observable<{ recipes: Recipe[] }> {
    return this.http.get<{ recipes: Recipe[] }>(this.apiUrl);
  }
  getRecipeById(id: number): Observable<Recipe> {
  return this.http.get<Recipe>(`${this.apiUrl}/${id}`);
}
}
