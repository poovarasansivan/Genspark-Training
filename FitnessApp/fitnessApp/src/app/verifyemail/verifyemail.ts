import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { LucideAngularModule, Dumbbell, View } from 'lucide-angular';
import { AuthService } from '../service/auth.service';
import { Popup } from "../popup/popup";

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
  selector: 'app-verifyemail',
  imports: [ReactiveFormsModule, CommonModule, LucideAngularModule, Popup],
  templateUrl: './verifyemail.html',
  styleUrl: './verifyemail.css',
})
export class Verifyemail {
  @ViewChild(Popup) toast!: Popup;

  DumbbellIcon = Dumbbell;
  verifyEmail = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email, emailPatternValidator()]),
  });

  constructor(private route: Router, private authService: AuthService) {}

  get email() {
    return this.verifyEmail.get('email')!;
  }

  verifyEmailAddress(): void {
    if (this.verifyEmail.invalid) {
      console.log('Form is invalid');
      alert('Please fill in all required fields correctly.');
      return;
    }
    
    const { email } = this.verifyEmail.value;
    this.authService.verifyEmail(email as string).subscribe({
      next: (response) => {
        console.log('Verification email sent successfully:', response);
        this.toast.display('Verification email sent successfully', 'success');
        setTimeout(() => {
          this.route.navigate(['/login']);
        }, 3000); 
      },
      error: (error) => {
        console.error('Error sending verification email:', error);
        this.toast.display('Failed to send verification email', 'error');
      },
    });
  }
}
