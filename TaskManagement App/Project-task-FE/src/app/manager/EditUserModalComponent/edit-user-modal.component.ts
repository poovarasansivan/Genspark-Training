import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserUpdateDto, UserResponseDto } from '../Create-User/user.types';
import { UserService } from '../Create-User/user.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-user-modal',
  templateUrl: './edit-user-modal.component.html',
  styleUrls: ['./edit-user-modal.component.css'],
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
})
export class EditUserModalComponent {
  @Input() user?: UserResponseDto;
  @Output() close = new EventEmitter<void>();
  @Output() userUpdated = new EventEmitter<void>();

  form: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private toastr: ToastrService
  ) {
    this.form = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      role: ['', Validators.required],
      password: ['']
    });
  }

  ngOnInit() {
  if (this.user) {
    this.form.patchValue(this.user);
  }
}

  submit() {
  if (this.form.invalid || !this.user) return; 

  this.loading = true;
  const updateDto: UserUpdateDto = this.form.value;

  this.userService.updateUser(this.user.id, updateDto).subscribe({
    next: () => {
      this.toastr.success('User updated successfully');
      this.userUpdated.emit();
      this.close.emit();
      this.loading = false;
    },
    error: err => {
      this.toastr.error(err?.error?.message || 'Update failed');
      this.loading = false;
    }
  });
}

}
