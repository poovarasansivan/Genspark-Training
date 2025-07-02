import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Menus } from './menu';
import { TokenService } from '../service/token.service';
import { ActivatedRoute } from '@angular/router';

describe('Menu', () => {
  let component: Menus;
  let fixture: ComponentFixture<Menus>;
  let tokenServiceSpy: jasmine.SpyObj<TokenService>;
  let ActivatedRoutespy: jasmine.SpyObj<ActivatedRoute>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Menus],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { queryParams: {} } },
        },
        {
          provide: TokenService,
          useValue: jasmine.createSpyObj('TokenService', ['getRole']),
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Menus);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
