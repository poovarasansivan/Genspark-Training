import { TestBed } from '@angular/core/testing';
import { App } from './app';
import { AuthService } from './auth/auth.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';

describe('App', () => {
  let authServiceMock: jasmine.SpyObj<AuthService>;
  let routerMock: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    authServiceMock = jasmine.createSpyObj('AuthService', ['getAccessToken', 'getRefreshToken', 'getUser', 'refreshToken']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    // Default behavior
    authServiceMock.getAccessToken.and.returnValue(null);
    authServiceMock.getRefreshToken.and.returnValue('dummy-refresh-token');
    authServiceMock.getUser.and.returnValue(null);
    authServiceMock.refreshToken.and.returnValue(of(true));

    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should call refreshToken if no accessToken but refreshToken exists', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges(); // triggers ngOnInit

    expect(authServiceMock.refreshToken).toHaveBeenCalled();
  });

  it('should navigate to login if token and user missing', () => {
    // Simulate no token and no refreshToken
    authServiceMock.getAccessToken.and.returnValue(null);
    authServiceMock.getRefreshToken.and.returnValue(null);
    authServiceMock.getUser.and.returnValue(null);

    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });


});
