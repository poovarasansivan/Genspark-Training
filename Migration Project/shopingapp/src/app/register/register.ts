import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { LucideAngularModule, ShoppingCart } from 'lucide-angular';
import { Router } from '@angular/router';
import { UserService } from '../services/user-service';

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  registerForm!: FormGroup;
  ShoppingCartIcon = ShoppingCart;
  constructor(private fb: FormBuilder, private route: Router, private userService:UserService) {}

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onSubmit(): void {
   if(this.registerForm.invalid) {
      console.log('Form is invalid');
      return;
    }
    const { name, username, email, password } = this.registerForm.value;
    console.log(this.registerForm.value);
    this.userService.addUser({ name, username, email, password }).subscribe({
      next: (response) => {
        console.log('Registration Successful');
        console.log(response);
        this.route.navigate(['/login']);
      },
      error: (error) => {
        console.log('Registration failed', error);
      },
    });
  }
}
