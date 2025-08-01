import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TaskService } from '../create-task/task.service';
import { TaskItemUpdateDto, TaskItemResponseDto, TaskState } from '../create-task/task.types';
import { UserService } from '../Create-User/user.service';
import { AuthService } from './../../auth/auth.service';
import { ToastrService } from 'ngx-toastr';
import { futureDateValidator } from '../date';

@Component({
  selector: 'app-task-edit-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  styleUrls: ['./task-edit-modal.css'],
  templateUrl: './task-edit-modal.html',
})
export class TaskEditModalComponent implements OnInit {
  @Input() task!: TaskItemResponseDto;
  @Output() close = new EventEmitter<void>();
  @Output() taskUpdated = new EventEmitter<void>();

  form!: FormGroup;
  taskState = TaskState;
  users: any[] = [];
  currentUser: any;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    private userService: UserService,
    private toastr: ToastrService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getUser();
    const safeDueDate = this.task.dueDate
      ? new Date(this.task.dueDate).toISOString().substring(0, 10)
      : '';

    this.form = this.fb.group({
      status: [this.task.status, Validators.required],
    });

    if (this.isManager()) {
      this.form.addControl('title', this.fb.control(this.task.title, Validators.required));
      this.form.addControl('description', this.fb.control(this.task.description, Validators.required));
      this.form.addControl('dueDate', this.fb.control(safeDueDate, [Validators.required, futureDateValidator]));
      this.form.addControl('assignedToId', this.fb.control(this.task.assignedToId, Validators.required));
      this.loadUsers();
    }
  }

  loadUsers(): void {
    this.userService.getAllUsers().subscribe({
      next: (res) => {
        const allUsers = res.data || res;
        this.users = allUsers.filter((u: any) => u.role === 'TeamMember');
      },
      error: (err) => {
        console.error('Failed to fetch users:', err);
      }
    });
  }

  submit(): void {
    const raw = this.form.getRawValue();
    const dto: TaskItemUpdateDto = this.isManager()
      ? raw
      : { status: raw.status }; 

    console.log('Submitting DTO:', dto);

    this.loading = true;
    this.taskService.updateTask(this.task.id, dto).subscribe({
      next: () => {
        this.toastr.success('Task updated successfully');
        this.taskUpdated.emit();
        this.close.emit();
        this.loading = false;
      },
      error: (err) => {
        this.toastr.error(err?.error?.message || 'Update failed');
        console.error('Update failed', err);
        this.loading = false;
      }
    });
  }

  isManager(): boolean {
    return this.currentUser?.role === 'Manager';
  }
}
