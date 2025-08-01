import { Component, Input, Output, EventEmitter } from '@angular/core';
import { TaskItemResponseDto } from '../create-task/task.types';
import { TaskService } from '../create-task/task.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-task-upload-modal',
  templateUrl: './task-upload-modal.component.html',
  imports: [CommonModule],
  styleUrls: ['./task-upload-modal.component.css'],
  standalone: true
})
export class TaskUploadModalComponent {
  @Input() task!: TaskItemResponseDto;
  @Output() close = new EventEmitter<void>();
  @Output() fileUploaded = new EventEmitter<void>();

  selectedFile: File | null = null;
  uploading = false;
  fileError: string = '';

  allowedTypes = [
    'image/png',
    'image/jpeg',
    'application/pdf',
    'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
  ];

  constructor(
    private taskService: TaskService,
    private toastr: ToastrService
  ) {}

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) return;

    if (!this.allowedTypes.includes(file.type)) {
      this.fileError = 'Only PNG, JPEG, PDF, and DOCX files are allowed.';
      this.selectedFile = null;
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      this.fileError = 'File must be smaller than 5 MB.';
      this.selectedFile = null;
      return;
    }

    this.fileError = '';
    this.selectedFile = file;
  }

  upload(): void {
    if (!this.selectedFile || !this.task) return;

    const formData = new FormData();
    formData.append('file', this.selectedFile);

    this.uploading = true;
    this.taskService.uploadFile(this.task.id, formData).subscribe({
      next: () => {
        this.toastr.success('File uploaded successfully');
        this.fileUploaded.emit();
        this.close.emit();
        this.uploading = false;
        this.selectedFile = null;
      },
      error: (err) => {
        this.toastr.error(err?.error?.message || 'Upload failed');
        this.uploading = false;
      }
    });
  }
}
