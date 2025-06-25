import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecipeCard } from './recipe-card';
import { Router } from '@angular/router';
import { Recipe } from '../../models/recipe';
import { RouterTestingModule } from '@angular/router/testing';
import { By } from '@angular/platform-browser';

describe('RecipeCard', () => {
  let component: RecipeCard;
  let fixture: ComponentFixture<RecipeCard>;
  let mockRouter: Router;

  
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
    await TestBed.configureTestingModule({
      imports: [RecipeCard, RouterTestingModule],
    }).compileComponents();

    fixture = TestBed.createComponent(RecipeCard);
    component = fixture.componentInstance;
    mockRouter = TestBed.inject(Router);
    component.recipe = mockRecipe;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should render recipe details', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Test Recipe');
    expect(compiled.textContent).toContain('Italian');
    expect(compiled.textContent).toContain('Medium');
    expect(compiled.textContent).toContain('4.5');
  });

  it('should call router.navigateByUrl',()=>{
    const spy = spyOn(mockRouter,'navigateByUrl');
    component.navigate(mockRecipe.id);
    expect(spy).toHaveBeenCalledWith('/recipes/1');
  })

  it('should call navigate() method when View button is clicked', () => {
    const spy = spyOn(component, 'navigate');
    const button = fixture.debugElement.query(By.css('button'));
    button.triggerEventHandler('click');
    expect(spy).toHaveBeenCalledWith(1);
  });

});
