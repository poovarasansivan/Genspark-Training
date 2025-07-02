import {
  ComponentFixture,
  fakeAsync,
  TestBed,
  tick,
} from '@angular/core/testing';

import { Login } from './login';
import { AuthService } from '../service/auth.service';
import { TokenService } from '../service/token.service';
import { Router } from '@angular/router';
import { Popup } from '../popup/popup';
import { of, throwError } from 'rxjs';
import { ReactiveFormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';

describe('Login', () => {
  let component: Login;
  let fixture: ComponentFixture<Login>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let tokenServiceSpy: jasmine.SpyObj<TokenService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['login']);
    tokenServiceSpy = jasmine.createSpyObj('TokenService', ['getRole']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [Login, Popup, ReactiveFormsModule, LucideAngularModule],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: TokenService, useValue: tokenServiceSpy },
        { provide: Router, useValue: routerSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;

    component.toast = jasmine.createSpyObj('Popup', ['display']);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should validate form inputs', () => {
    component.loginForm.setValue({ email: '', password: '' });
    expect(component.loginForm.invalid).toBeTrue();
  });

  it('should validate email format', () => {
    component.loginForm.setValue({
      email: 'invalid-email',
      password: 'password123',
    });
    expect(component.loginForm.invalid).toBeTrue();
  });

  it('should display toast on invalid form submission', () => {
    spyOn(component.toast, 'display');
    component.loginForm.setValue({ email: '', password: '' });
    component.login();

    expect(component.toast.display).toHaveBeenCalledWith(
      'Please fill in all required fields correctly.',
      'error'
    );
  });

  it('should navigate to reset password on resetPassword()', () => {
    component.resetPassword();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/verify-email']);
  });
});
function done() {
  throw new Error('Function not implemented.');
}
