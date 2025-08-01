import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../manager/create-task/task.service';
import { formatDistanceToNow } from 'date-fns';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-recent-tasks-table',
  standalone: true,
  imports: [CommonModule,RouterLink],
  templateUrl: './recent-tasks-table.html',
  styleUrls: ['./recent-tasks-table.css']
})
export class RecentTasksTableComponent implements OnInit {
  user = JSON.parse(sessionStorage.getItem('user') || '{}');
  recentTasks: any[] = [];
  loading = true;

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.taskService.getAllTasks(1, 1000).subscribe({
      next: (res) => {
        const tasks = res.data || [];
        this.recentTasks = tasks
          .sort((a: any, b: any) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
          .slice(0, 5);
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading recent tasks', err);
        this.loading = false;
      }
    });
  }

   isManager() {
  return this.user?.role === 'Manager';
}

isTeamMember() {
  return this.user?.role === 'TeamMember';
}

  getTimeAgo(date: string): string {
    return formatDistanceToNow(new Date(date), { addSuffix: true });
  }
}
