import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { LucideAngularModule, ShoppingCart } from 'lucide-angular';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth-service';
import { Toast } from '../toast/toast';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule, Toast],
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
})
export class LoginComponent implements OnInit {
  @ViewChild(Toast) toast!: Toast;

  loginForm!: FormGroup;
  ShoppingCartIcon = ShoppingCart;
  constructor(
    private fb: FormBuilder,
    private route: Router,
    private authservice: AuthService
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.toast.display('Form is Invalid', 'error');
      console.log('Form is invalid');
      return;
    }

    const { username, password } = this.loginForm.value;
    console.log(this.loginForm.value);
    this.authservice.login(username, password).subscribe({
      next: (response) => {
        this.toast.display('Login Successfull', 'success');
        localStorage.setItem('Access-Token', response.token);
        localStorage.setItem('Refresh-Token', response.refreshToken);
        this.route.navigate(['/home']);
      },
      error: (error) => {
        this.toast.display('Login Failed', 'error');
        console.log('Login failed', error);
      },
    });
  }
}
