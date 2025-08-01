import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../auth.service';
import { LoginRequestDto } from '../auth.types';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { SignalRService } from '../signalr.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  loginForm: FormGroup;
  showPassword = false;
  loading = false;
  errorMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService,
    private signalRService: SignalRService 
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

 onSubmit(): void {
  if (this.loginForm.invalid) return;

  const payload: LoginRequestDto = this.loginForm.value;
  this.loading = true;

  this.authService.login(payload).subscribe({
    next: (res) => {
      this.loading = false;

      if (res.success) {
        sessionStorage.clear();
        this.authService['setSession'](res.data); 
        const user = this.authService.getUser();
        console.log('[SignalR] Frontend userId:', user?.id);
        this.signalRService.startConnection(); 
        this.toastr.success('Login successful. Welcome back!');
        this.router.navigate(['/dashboard']);
      } else {
        this.errorMessage = res.message;
      }
    },
    error: (err) => {
      this.loading = false;
      this.errorMessage = err?.error?.message || 'Login failed.';
      this.toastr.error(this.errorMessage);
    }
  });
}

}
