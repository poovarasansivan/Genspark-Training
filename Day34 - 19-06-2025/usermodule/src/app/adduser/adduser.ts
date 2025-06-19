import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormGroup,
  FormControl,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import {
  bannedWordsValidator,
  emailPatternValidator,
  passwordMatchValidator,
} from '../validator/customvalidator';
import { User } from '../models/user';
import { UserService } from '../service/user.service';
import { Router } from '@angular/router';
import { Notification } from '../notification/notification';

@Component({
  selector: 'app-adduser',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, Notification],
  templateUrl: './adduser.html',
  styleUrl: './adduser.css',
})
export class Adduser implements OnInit {
  userForm!: FormGroup;
  showError = false;
  showSuccess = false;
  successMessage = '';
  errorMessgae = '';

  constructor(private userService: UserService, private route: Router) {}

  ngOnInit(): void {
    this.userForm = new FormGroup(
      {
        username: new FormControl('', [
          Validators.required,
          bannedWordsValidator(['Admin', 'User', 'Guest', 'Root']),
        ]),
        email: new FormControl('', [
          Validators.required,
          Validators.email,
          emailPatternValidator(),
        ]),
        password: new FormControl('', [
          Validators.required,
          Validators.pattern(/^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).{8,}$/),
        ]),
        confirmPassword: new FormControl('', Validators.required),
        role: new FormControl('', Validators.required),
      },
      {
        validators: passwordMatchValidator(),
      }
    );
  }

  onSubmit(): void {
    if (this.userForm.valid) {
      const { username, email, password, role } = this.userForm.value;
      const user: User = { username, email, password, role };
      this.userService.addUser(user);
      this.successMessage = 'User Added Successfully';
      this.showError = false;
      this.showSuccess = true;
      setTimeout(() => this.route.navigateByUrl('/home'), 4000);
    } else {
      this.errorMessgae = 'Failed to Add User!';
      this.showError = true;
      this.showSuccess = false;
      setTimeout(() => this.route.navigateByUrl('/home'), 4000);
    }
  }
}
