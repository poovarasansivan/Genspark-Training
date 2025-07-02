import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Forgotpassword, passwordMatchValidator } from './forgotpassword';
import { AuthService } from '../service/auth.service';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { FormGroup } from '@angular/forms';
import { of, throwError } from 'rxjs';

describe('Forgotpassword', () => {
  let component: Forgotpassword;
  let fixture: ComponentFixture<Forgotpassword>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let activatedRouteStub: Partial<ActivatedRoute>;
  let routeSpy: jasmine.SpyObj<ActivatedRoute>;

  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['resetPassword']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    routeSpy = {
      snapshot: {
        queryParams: { token: 'mock-token' },
      },
    } as any;

    await TestBed.configureTestingModule({
      imports: [Forgotpassword],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: routeSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Forgotpassword);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  describe('passwordMatchValidator', () => {
    it('should return null if passwords match', () => {
      const form = new FormGroup({
        password: component.password,
        confirmPassword: component.confirmPassword,
      });
      form.get('password')?.setValue('123456');
      form.get('confirmPassword')?.setValue('123456');
      expect(passwordMatchValidator(form)).toBeNull();
    });

    it('should return error if passwords do not match', () => {
      const form = new FormGroup({
        password: component.password,
        confirmPassword: component.confirmPassword,
      });
      form.get('password')?.setValue('abc123');
      form.get('confirmPassword')?.setValue('xyz456');
      expect(passwordMatchValidator(form)).toEqual({ passwordMismatch: true });
    });
  });

  it('should not reset if form is invalid', () => {
    spyOn(console, 'log');

    component.passwordForm.setValue({
      password: '',
      confirmPassword: '',
    });
    component.reset();

    expect(authServiceSpy.resetPassword).not.toHaveBeenCalled();
  });
});
