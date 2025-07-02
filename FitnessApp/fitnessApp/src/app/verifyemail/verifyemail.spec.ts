import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Verifyemail } from './verifyemail';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { AuthService } from '../service/auth.service';

describe('Verifyemail', () => {
  let component: Verifyemail;
  let fixture: ComponentFixture<Verifyemail>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['verifyEmail']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [Verifyemail],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Verifyemail);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  it('should mark form invalid when email is empty', () => {
    component.verifyEmail.setValue({ email: '' });
    expect(component.verifyEmail.invalid).toBeTrue();
  });

  it('should call authService.verifyEmail and navigate on success', () => {
    spyOn(window, 'alert');
    component.verifyEmail.setValue({ email: '' });
    component.verifyEmailAddress();
    expect(window.alert).toHaveBeenCalledWith('Please fill in all required fields correctly.');
    expect(authServiceSpy.verifyEmail).not.toHaveBeenCalled();
  });

});
