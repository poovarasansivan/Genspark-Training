import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecipePage } from './recipe-page';
import { Recipe } from '../../models/recipe';
import { RecipeService } from '../../services/recipe';
import { RecipeCard } from '../../components/recipe-card/recipe-card';
import { CommonModule } from '@angular/common';
import {
  RouterTestingHarness,
  RouterTestingModule,
} from '@angular/router/testing';
import { of, throwError } from 'rxjs';

describe('RecipePage', () => {
  let component: RecipePage;
  let fixture: ComponentFixture<RecipePage>;
  let recipeService: jasmine.SpyObj<RecipeService>;

  let mockRecipe: Recipe = {
    id: 1,
    name: 'Test Recipe',
    image: 'https://example.com/image.jpg',
    cuisine: 'Italian',
    difficulty: 'Medium',
    rating: 4.5,
    ingredients: ['Salt', 'Tomato'],
    instructions: ['Boil', 'Serve'],
  };

  beforeEach(async () => {
    const recipeServiceSpy = jasmine.createSpyObj('RecipeService', [
      'getRecipes',
    ]);

    await TestBed.configureTestingModule({
      imports: [RecipePage, RecipeCard, CommonModule, RouterTestingModule],
      providers: [
        {
          provide: RecipeService,
          useValue: recipeServiceSpy,
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(RecipePage);
    component = fixture.componentInstance;
    recipeService = TestBed.inject(
      RecipeService
    ) as jasmine.SpyObj<RecipeService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch recipe on init succes', () => {
    recipeService.getRecipes.and.returnValue(of({ recipes: [mockRecipe] }));
    component.ngOnInit();

    expect(component.recipes.length).toBe(1);
    expect(component.isLoading).toBeFalse();
    expect(component.error).toBe('');
  });

  it('should handle error on init', () => {
    recipeService.getRecipes.and.returnValue(throwError(() => new Error('API failed')));

    component.ngOnInit();

    expect(component.recipes.length).toBe(0);
    expect(component.isLoading).toBeFalse();
    expect(component.error).toBe('Failed to load recipes.');
  });
});
