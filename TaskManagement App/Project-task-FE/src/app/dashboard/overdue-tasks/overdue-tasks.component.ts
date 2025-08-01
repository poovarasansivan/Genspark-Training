import { Component, OnInit } from '@angular/core';
import { TaskService } from '../../manager/create-task/task.service';
import { TaskItemResponseDto } from '../../manager/create-task/task.types';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-overdue-tasks',
  templateUrl: './overdue-tasks.component.html',
  imports: [CommonModule],
  standalone: true,
  styleUrls: ['./overdue-tasks.component.css']
})
export class OverdueTasksComponent implements OnInit {
   overdueTasks: TaskItemResponseDto[] = [];
   paginatedTasks: TaskItemResponseDto[] = [];
   loading = true;
   currentUserRole: string = ''; 

  currentPage = 1;
  tasksPerPage = 3;
  totalPages = 0;

  constructor(private taskService: TaskService ,private  authService : AuthService) {}

  ngOnInit(): void {
    this.currentUserRole = this.authService.getUser()?.role|| '';
    this.taskService.getAllTasks(1, 1000).subscribe({
      next: (res) => {
        const today = new Date();
        this.overdueTasks = res.data.filter(task => new Date(task.dueDate || '') < today && task.status !== 'Done');
        this.totalPages = Math.ceil(this.overdueTasks.length / this.tasksPerPage);
        this.updatePaginatedTasks();
        this.loading = false;
      },
      error: () => (this.loading = false)
    });
  }

  updatePaginatedTasks() {
    const start = (this.currentPage - 1) * this.tasksPerPage;
    const end = start + this.tasksPerPage;
    this.paginatedTasks = this.overdueTasks.slice(start, end);
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.updatePaginatedTasks();
    }
  }

  previousPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.updatePaginatedTasks();
    }
  }

}


