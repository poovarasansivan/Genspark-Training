import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from './login';
import { ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;

   beforeEach(async () => {
  authServiceSpy = jasmine.createSpyObj('AuthService', [
    'login',
    'getUser',
    'setSession',
    'getUserId',
    'getAccessToken',    
    'getRefreshToken',     
    'refreshToken',        
    'logout'           
  ]);

  authServiceSpy.getAccessToken.and.returnValue('mocked-token'); 

  routerSpy = jasmine.createSpyObj('Router', ['navigate']);

  await TestBed.configureTestingModule({
    imports: [ReactiveFormsModule, LoginComponent],
    providers: [
      { provide: AuthService, useValue: authServiceSpy },
      { provide: Router, useValue: routerSpy },
      {
        provide: ToastrService,
        useValue: jasmine.createSpyObj('ToastrService', ['success', 'error'])
      }
    ]
  }).compileComponents();

  fixture = TestBed.createComponent(LoginComponent);
  component = fixture.componentInstance;
  fixture.detectChanges();
});

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show error on invalid login', () => {
    component.loginForm.setValue({ email: '', password: '' });
    component.onSubmit();
    expect(component.loading).toBeFalse();
  });

  it('should call AuthService.login on submit', () => {
    component.loginForm.setValue({ email: 'test@test.com', password: '123456' });
    authServiceSpy.login.and.returnValue(of({ success: true, data: { user: { id: '1' } } }));
    component.onSubmit();
    expect(authServiceSpy.login).toHaveBeenCalled();
  });

  it('should show error message on login failure', () => {
    component.loginForm.setValue({ email: 'test@test.com', password: 'wrong' });
    authServiceSpy.login.and.returnValue(throwError({ error: { message: 'Login failed.' } }));
    component.onSubmit();
    expect(component.errorMessage).toBe('Login failed.');
  });
});