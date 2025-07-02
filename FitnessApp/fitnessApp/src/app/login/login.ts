import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import {
  FormGroup,
  FormControl,
  Validators,
  ReactiveFormsModule,
  AbstractControl,
  ValidationErrors,
  ValidatorFn,
} from '@angular/forms';
import { Router } from '@angular/router';
import { LucideAngularModule, Dumbbell } from 'lucide-angular';
import { AuthService } from '../service/auth.service';
import { Popup } from '../popup/popup';
import { TokenService } from '../service/token.service';

export function emailPatternValidator(
  pattern: RegExp = /^[a-zA-Z0-9._%+-]+@(gmail|outlook)\.com$/
): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const email = control.value;
    if (!email) return null;
    return pattern.test(email) ? null : { emailPatternValidator: true };
  };
}

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, CommonModule, LucideAngularModule, Popup],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  @ViewChild(Popup) toast!: Popup;

  DumbbellIcon = Dumbbell;

  loginForm = new FormGroup({
    email: new FormControl('', [
      Validators.required,
      Validators.email,
      emailPatternValidator(),
    ]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(8),
      Validators.maxLength(20),
      Validators.pattern(
        /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$/
      ),
    ]),
  });

  constructor(
    private router: Router,
    private authService: AuthService,
    private tokenService: TokenService
  ) {}

  get email() {
    return this.loginForm.get('email')!;
  }

  get password() {
    return this.loginForm.get('password')!;
  }

  loggedInUserRole: string = '';

  login(): void {
    if (this.loginForm.invalid) {
      console.log('Form is invalid');
      this.toast.display(
        'Please fill in all required fields correctly.',
        'error'
      );
      return;
    }

    const { email, password } = this.loginForm.value;

    this.authService.login(email!, password!).subscribe({
      next: (response) => {
        console.log('Login successful', response);
        this.toast.display('Login successful!', 'success');

        localStorage.setItem('token', response.token);

        localStorage.setItem('refreshToken', response.refreshToken);
        this.loggedInUserRole = this.tokenService.getRole() || '';
        setTimeout(() => {
          if (this.loggedInUserRole === 'Admin') {
            this.router.navigate(['/home']);
          } else if (this.loggedInUserRole === 'Coach') {
            this.router.navigate(['/coach-home']);
          } else {
            this.router.navigate(['/client-home']);
          }
        }, 4000);
      },
      error: (error) => {
        console.error('Login failed', error);
        this.toast.display(
          'Login failed. Please check your credentials and try again.',
          'error'
        );
      },
    });
  }

  resetPassword(): void {
    this.router.navigate(['/verify-email']);
  }
}
