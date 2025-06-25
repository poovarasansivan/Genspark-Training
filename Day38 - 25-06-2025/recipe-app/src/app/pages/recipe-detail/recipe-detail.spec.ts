import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecipeDetail } from './recipe-detail';
import { RecipeService } from '../../services/recipe';
import { Recipe } from '../../models/recipe';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';

describe('RecipeDetail', () => {
  let component: RecipeDetail;
  let fixture: ComponentFixture<RecipeDetail>;
  let recipeService: jasmine.SpyObj<RecipeService>;

  const mockRecipe: Recipe = {
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
    recipeService =  jasmine.createSpyObj('RecipeService', ['getRecipeById']);

    await TestBed.configureTestingModule({
      imports: [RecipeDetail,CommonModule],
      providers: [
        {provide:RecipeService,useValue:recipeService},
        {provide:ActivatedRoute,
          useValue:{
            snapshot:{
              paramMap:{
                get:(key:string)=>'1'
              }
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RecipeDetail);
    component = fixture.componentInstance;
  });

   it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch recipe by ID on init (success)', () => {
    recipeService.getRecipeById.and.returnValue(of(mockRecipe));
    component.ngOnInit();

    expect(recipeService.getRecipeById).toHaveBeenCalledWith(1);
    expect(component.recipe).toEqual(mockRecipe);
    expect(component.isLoading).toBeFalse();
    expect(component.error).toBe('');
  });

  it('should handle error during recipe fetch', () => {
    recipeService.getRecipeById.and.returnValue(
      throwError(() => new Error('404 not found'))
    );

    component.ngOnInit();

    expect(recipeService.getRecipeById).toHaveBeenCalledWith(1);
    expect(component.recipe).toBeUndefined();
    expect(component.isLoading).toBeFalse();
    expect(component.error).toBe('Could not load recipe');
  });
});