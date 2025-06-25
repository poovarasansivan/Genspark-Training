import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { RecipeService } from '../services/recipe';
import { Recipe } from '../models/recipe';

describe('RecipeService', () => {
  let service: RecipeService;
  let httpMock: HttpTestingController;

  const dummyRecipes: Recipe[] = [
    {
      id: 1,
      name: 'Pasta',
      image: 'https://example.com/pasta.jpg',
      cuisine: 'Italian',
      difficulty: 'Medium',
      rating: 4.2,
      ingredients: ['Pasta', 'Salt'],
      instructions: ['Boil pasta', 'Add salt'],
    },
    {
      id: 2,
      name: 'Pizza',
      image: 'https://example.com/pizza.jpg',
      cuisine: 'Italian',
      difficulty: 'Hard',
      rating: 4.8,
      ingredients: ['Flour', 'Cheese'],
      instructions: ['Bake dough', 'Add cheese'],
    },
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [RecipeService],
    });

    service = TestBed.inject(RecipeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Verifies no unmatched requests
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all recipes', () => {
    service.getRecipes().subscribe((res) => {
      expect(res.recipes.length).toBe(2);
      expect(res.recipes).toEqual(dummyRecipes);
    });

    const req = httpMock.expectOne('https://dummyjson.com/recipes');
    expect(req.request.method).toBe('GET');
    req.flush({ recipes: dummyRecipes });
  });

  it('should fetch recipe by ID', () => {
    const targetRecipe = dummyRecipes[1];

    service.getRecipeById(2).subscribe((recipe) => {
      expect(recipe).toEqual(targetRecipe);
    });

    const req = httpMock.expectOne('https://dummyjson.com/recipes/2');
    expect(req.request.method).toBe('GET');
    req.flush(targetRecipe);
  });
});
