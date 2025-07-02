import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
  ValidationErrors,
  AbstractControl,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../service/auth.service';
import { Popup } from '../popup/popup';

export function passwordMatchValidator(
  form: AbstractControl
): ValidationErrors | null {
  const password = form.get('password')?.value;
  const confirmPassword = form.get('confirmPassword')?.value;
  return password === confirmPassword ? null : { passwordMismatch: true };
}

export function complexPasswordValidator(
  control: AbstractControl
): ValidationErrors | null {
  const value = control.value;
  if (!value) {
    return null;
  }
  const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,20}$/;
  return regex.test(value) ? null : { complexPassword: true };
}

@Component({
  selector: 'app-forgotpassword',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, FormsModule, Popup],
  templateUrl: './forgotpassword.html',
  styleUrl: './forgotpassword.css',
})
export class Forgotpassword {
  @ViewChild(Popup) toast!: Popup;
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {}

  passwordForm = new FormGroup(
    {
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(20),
        complexPasswordValidator,
      ]),
      confirmPassword: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(20),
      ]),
    },
    { validators: passwordMatchValidator }
  );

  get password() {
    return this.passwordForm.get('password')!;
  }

  get confirmPassword() {
    return this.passwordForm.get('confirmPassword')!;
  }

  reset(): void {
    if (this.passwordForm.valid) {
      const token = this.route.snapshot.queryParamMap.get('token');
      if (!token) {
        this.toast.display('Token is missing from the URL', 'error');
        return;
      }

      console.log('Resetting password', this.passwordForm.value);

      this.authService.resetPassword(token, this.password.value!).subscribe({
        next: (response) => {
          this.toast.display('Password reset successfully', 'success');
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 3000);
        },
        error: (error) => {
          console.error('Error resetting password:', error);
          this.toast.display('Failed to reset password', 'error');
        },
      });
    } else {
      this.toast.display(
        'Please fill in all required fields correctly.',
        'error'
      );
    }
  }
}
