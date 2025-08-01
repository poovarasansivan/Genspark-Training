import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { TaskFileDto } from '../create-task/task.types';
import { TaskService } from '../create-task/task.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-task-files-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './task-files-modal.component.html',
  styleUrls: ['./task-files-modal.component.css'],
  providers: [DatePipe]
})
export class TaskFilesModalComponent implements OnInit {
  @Input() taskId!: string;
  @Output() close = new EventEmitter<void>();

  files: TaskFileDto[] = [];
  loading = false;

  constructor(private taskService: TaskService, private toastr: ToastrService) {}

  ngOnInit(): void {
    this.loading = true;
    this.taskService.getFiles(this.taskId).subscribe({
      next: (res) => {
        this.files = res.data || [];
        this.loading = false;
      },
      error: (err) => {
      const message = err?.error?.errors?.Error?.[0] || err?.error?.message || 'Failed to fetch files';
      this.toastr.error(message);
      this.loading = false;
    }
  });
  }

  download(file: TaskFileDto): void {
    this.taskService.downloadFile(file.id).subscribe({
      next: (blob) => {
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = file.fileName;
        link.click();
        URL.revokeObjectURL(link.href);
      },
      error: () => {
        this.toastr.error('Download failed');
      }
    });
  }
}
