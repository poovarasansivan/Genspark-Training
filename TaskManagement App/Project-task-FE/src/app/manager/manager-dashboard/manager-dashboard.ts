import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { TaskListComponent } from '../task-list/task-list.component';
import { TaskDistributionDonutChartComponent } from '../../charts/task-distribution-donut-chart/task-distribution-donut-chart';
import { TasksPerMemberBarChartComponent } from '../../charts/tasks-per-member-bar-chart/tasks-per-member-bar-chart';
import { RecentTasksTableComponent } from '../../dashboard/recent-tasks-table/recent-tasks-table';
import { UpcomingTasksComponent } from '../../dashboard/upcoming-tasks/upcoming-tasks.component';
import { OverdueTasksComponent } from '../../dashboard/overdue-tasks/overdue-tasks.component';
import { MyTaskDeadlineBarChartComponent } from '../../charts/task-deadline-bar-chart/my-task-deadline-bar-chart';

@Component({
  selector: 'app-manager-dashboard',
  imports: [CommonModule,TaskDistributionDonutChartComponent,TasksPerMemberBarChartComponent,RecentTasksTableComponent,UpcomingTasksComponent,OverdueTasksComponent,MyTaskDeadlineBarChartComponent],
  templateUrl: './manager-dashboard.html',
  styleUrl: './manager-dashboard.css'
})
export class ManagerDashboard {
user = JSON.parse(sessionStorage.getItem('user') || '{}');


personalizedQuote = {
  pre: '',
  name: '',
  post: ''
};

quotes: string[] = [
  "Let's make today count, {{name}}!",
  "Keep pushing forward, {{name}}!",
  "Every task completed is a step closer, {{name}}.",
  "You're capable of amazing things, {{name}}.",
  "Lead with purpose, {{name}}.",
  "Small efforts build big results, {{name}}!",
  "Your team is counting on your brilliance, {{name}}.",
  "Make it a masterpiece of a day, {{name}}!"
];

ngOnInit(): void {
  if (this.user?.fullName) {
    const randomQuote = this.quotes[Math.floor(Math.random() * this.quotes.length)];
    const [pre, post] = randomQuote.split('{name}');
    const firstName = this.user.fullName.split(' ')[0];

    this.personalizedQuote = {
      pre: pre.trim(),
      name: firstName,
      post: post.trim()
    };
  }
}

 isManager() {
  return this.user?.role === 'Manager';
}

isTeamMember() {
  return this.user?.role === 'TeamMember';
}
}
