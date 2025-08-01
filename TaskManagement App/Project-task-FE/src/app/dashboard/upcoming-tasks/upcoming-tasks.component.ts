import { Component, OnInit } from '@angular/core';
import { TaskService } from '../../manager/create-task/task.service';
import { TaskItemResponseDto } from '../../manager/create-task/task.types';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-upcoming-tasks',
  templateUrl: './upcoming-tasks.component.html',
  imports: [FormsModule,CommonModule,RouterLink],
  styleUrls: ['./upcoming-tasks.component.css']
})
export class UpcomingTasksComponent implements OnInit {
user = JSON.parse(sessionStorage.getItem('user') || '{}');
  upcomingTasks: TaskItemResponseDto[] = [];
  paginatedTasks: TaskItemResponseDto[] = [];
  loading = true;

   page = 0;
  pageSize = 3;

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.taskService.getAllTasks(1, 1000).subscribe({
      next: (tasks) => {
        const today = new Date();
        this.upcomingTasks = tasks.data
  .filter(task => {
    const due = new Date(task.dueDate ?? '');
    return due > today;
  })
  .sort((a, b) => new Date(a.dueDate ?? '').getTime() - new Date(b.dueDate ?? '').getTime());

        this.updatePaginatedTasks();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  updatePaginatedTasks(): void {
    const start = this.page * this.pageSize;
    const end = start + this.pageSize;
    this.paginatedTasks = this.upcomingTasks.slice(start, end);
  }

  nextPage(): void {
    if ((this.page + 1) * this.pageSize < this.upcomingTasks.length) {
      this.page++;
      this.updatePaginatedTasks();
    }
  }

  get totalPages(): number {
  return Math.ceil(this.upcomingTasks.length / this.pageSize);
}

  prevPage(): void {
    if (this.page > 0) {
      this.page--;
      this.updatePaginatedTasks();
    }
  }
    isManager() {
  return this.user?.role === 'Manager';
}

isTeamMember() {
  return this.user?.role === 'TeamMember';
}
}