import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { TaskService } from '../create-task/task.service';
import { TaskStatusLogResponseDto } from '../create-task/task.types';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-task-status-log-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './task-status-log-modal.component.html',
  styleUrls: ['./task-status-log-modal.component.css']
})
export class TaskStatusLogModalComponent implements OnInit {
  @Input() taskId!: string;
  @Output() close = new EventEmitter<void>();

  logs: TaskStatusLogResponseDto[] = [];
  loading = false;

  constructor(private taskService: TaskService, private toastr: ToastrService) {}

  ngOnInit(): void {
    this.loading = true;
    this.taskService.getTaskStatusLogs(this.taskId).subscribe({
      next: (res) => {
        this.logs = res.data ?? [];
        this.loading = false;
      },
      error: (err) => {
        this.toastr.error(err?.error?.message || 'Failed to fetch logs');
        this.loading = false;
      }
    });
  }
}
