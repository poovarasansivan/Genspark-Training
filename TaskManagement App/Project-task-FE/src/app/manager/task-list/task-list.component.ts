import { Component, OnInit } from '@angular/core';
import { TaskItemResponseDto } from '../create-task/task.types';
import { TaskService } from '../create-task/task.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { TaskEditModalComponent } from '../task-edit-modal/task-edit-modal';
import { TaskUploadModalComponent } from '../task-upload/task-upload-modal.component';
import { TaskFilesModalComponent } from '../view-files/task-files-modal.component';
import { TaskStatusLogModalComponent } from '../task-status-log-modal/task-status-log-modal.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-task-list',
  templateUrl: './task-list.component.html',
  imports: [
    CommonModule,
    TaskEditModalComponent,
    TaskUploadModalComponent,
    TaskFilesModalComponent,
    TaskStatusLogModalComponent,
    FormsModule,
  ],
  styleUrls: ['./task-list.component.css'],
})
export class TaskListComponent implements OnInit {
  tasks: TaskItemResponseDto[] = [];
  loading = false;

  selectedTaskToEdit?: TaskItemResponseDto;
  selectedTaskToUpload?: TaskItemResponseDto;
  selectedTaskId = '';
  selectedTaskIdForLogs = '';
  draftTaskExpirysInDays: number = 0;
  showUploadModal = false;
  showFilesModal = false;
  showLogModal = false;

  currentUser: any;

  searchTitle: string = '';
  searchDueDate: string = '';
  selectedFilter: string = 'All';

  page = 1;
  pageSize = 6;
  totalPages = 0;

  constructor(
    private taskService: TaskService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    const userData = sessionStorage.getItem('user');
    if (userData) {
      this.currentUser = JSON.parse(userData);
    }
    this.fetchTasks();
  }

  getInitials(name: string): string {
    const words = name.trim().split(' ');
    return words
      .map((word) => word[0])
      .join('')
      .toUpperCase();
  }

  deleteTask(id: string) {
    if (confirm('Are you sure you want to delete this task?')) {
      this.taskService.deleteTask(id).subscribe({
        next: () => {
          this.toastr.success('Task deleted successfully');
          this.fetchTasks();
        },
        error: (err) => {
          this.toastr.error(err?.error?.message || 'Failed to delete task');
        },
      });
    }
  }

  openEdit(task: TaskItemResponseDto): void {
    this.selectedTaskToEdit = task;
    document.body.classList.add('modal-open');
  }

  closeEdit(): void {
    this.selectedTaskToEdit = undefined;
    document.body.classList.remove('modal-open');
    this.fetchTasks();
  }

  openFilesModal(taskId: string) {
    this.selectedTaskId = taskId;
    this.showFilesModal = true;
    document.body.classList.add('modal-open');
  }

  closeFilesModal(): void {
    this.showFilesModal = false;
    this.selectedTaskId = '';
    document.body.classList.remove('modal-open');
  }

  openUploadModal(task: TaskItemResponseDto) {
    this.selectedTaskToUpload = task;
    document.body.classList.add('modal-open');
    this.showUploadModal = true;
  }

  closeUploadModal(): void {
    this.showUploadModal = false;
    this.selectedTaskToUpload = undefined;
    document.body.classList.remove('modal-open');
  }

  openLogModal(taskId: string): void {
    this.selectedTaskIdForLogs = taskId;
    this.showLogModal = true;
    document.body.style.overflow = 'hidden';
  }

  closeLogModal(): void {
    this.showLogModal = false;
    this.selectedTaskIdForLogs = '';
    document.body.style.overflow = '';
  }

  searchTasks(): void {
    this.page = 1;
    this.fetchTasks();
  }

  clearSearch(): void {
    this.searchTitle = '';
    this.searchDueDate = '';
    this.page = 1;
    this.fetchTasks();
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.page = page;
    this.fetchTasks();
  }

  getPageRange(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  filterTasks(state: string) {
    this.selectedFilter = state;
    this.page = 1;
    this.fetchTasks();
  }

  fetchTasks(): void {
    this.loading = true;

    const status = this.selectedFilter === 'All' ? '' : this.selectedFilter;

    this.taskService
      .getAllTasks(
        this.page,
        this.pageSize,
        status,
        this.searchTitle,
        this.searchDueDate
      )
      .subscribe({
        next: (res) => {
          this.tasks = res.data.map((task) => {
            if (task.status === 'Draft' && task.createdAt) {
              const createdDate = new Date(task.createdAt);
              const now = new Date();
              const diffInDays = Math.floor(
                (now.getTime() - createdDate.getTime()) / (1000 * 3600 * 24)
              );
              const daysRemaining = 3 - diffInDays;

              return {
                ...task,
                daysUntilExpiry: daysRemaining > 0 ? daysRemaining : 0,
              };
            }
            return {
              ...task,
              daysSinceCreation: null,
              daysUntilExpiry: null,
            };
          });

          this.totalPages = res.pagination.totalPages;

          this.loading = false;
        },
        error: (err) => {
          this.tasks = [];
          this.totalPages = 0;
          this.toastr.error(err?.error?.message || 'No tasks Found');
          this.loading = false;
        },
      });
  }
}
