import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TaskService } from './task.service';
import { TaskItemRequestDto, TaskState } from './task.types';
import {UserResponseDto} from '../Create-User/user.types'
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../Create-User/user.service'; 
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-task',
  standalone: true,
  templateUrl: './create-task.html',
  styleUrls: ['./create-task.css'],
  imports: [CommonModule, ReactiveFormsModule],
})
export class CreateTaskComponent implements OnInit {
  taskForm: FormGroup;
  loading = false;
  users: UserResponseDto[] = [];
  today: string;

  statuses = Object.values(TaskState);

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    private userService: UserService,
    private toastr: ToastrService,
    private router: Router
  ) {
    const now = new Date();
    this.today = now.toISOString().split('T')[0];
    this.taskForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required, Validators.minLength(10)]],
      dueDate: [null, [Validators.required, this.futureDateValidator()]],
      assignedToId: ['', Validators.required],
      status: [TaskState.ToDo, Validators.required]
    });
  }

  ngOnInit(): void {
    this.userService.getAllTeamMembers().subscribe({
    next: res => {
      this.users = res.data;
    },
    error: () => {
      this.toastr.error('Failed to load team members');
    }
  });
  }


  futureDateValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const selectedDate = new Date(control.value);
    const today = new Date();
    today.setHours(0, 0, 0, 0); 

    if (selectedDate < today) {
      return { pastDate: true };
    }
    return null;
  };
}


  submit() {
    if (this.taskForm.invalid) return;

    const rawFormValue = this.taskForm.value;

    this.loading = true;
    const dto: TaskItemRequestDto = {
      title: this.taskForm.value.title,
      description: this.taskForm.value.description,
      status: this.taskForm.value.status,
      dueDate: rawFormValue.dueDate
      ? new Date(rawFormValue.dueDate).toISOString() 
      : undefined,
      assignedToId: this.taskForm.value.assignedToId
    };

    this.taskService.createTask(dto).subscribe({
      next: () => {
        this.toastr.success('Task created successfully');
        this.router.navigate(['/tasks']);
      },
      error: err => {
        this.toastr.error(err?.error?.message || 'Failed to create task');
        this.loading = false;
      }
    });
  }

  draftTask(){

    const rawFormValue = this.taskForm.value;

    this.loading = true;
    const dto: TaskItemRequestDto = {
      title: this.taskForm.value.title,
      description: this.taskForm.value.description,
      status: TaskState.Draft, 
      dueDate: rawFormValue.dueDate
        ? new Date(rawFormValue.dueDate).toISOString() 
        : undefined,
      assignedToId: this.taskForm.value.assignedToId
    };

    this.taskService.draftTask(dto).subscribe({
      next: () => {
        this.toastr.success('Task drafted successfully');
        this.router.navigate(['/tasks']);
      },
      error: err => {
        this.toastr.error(err?.error?.message || 'Failed to draft task');
        this.loading = false;
      }
    });
  }
}
