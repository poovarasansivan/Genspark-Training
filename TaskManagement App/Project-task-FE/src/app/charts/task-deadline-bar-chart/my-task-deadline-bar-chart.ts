import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartData, ChartType, ChartOptions } from 'chart.js';
import { NgChartsModule, BaseChartDirective } from 'ng2-charts';
import { TaskService } from '../../manager/create-task/task.service';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-my-task-deadline-bar-chart',
  standalone: true,
  imports: [CommonModule, NgChartsModule],
  templateUrl: './my-task-deadline-bar-chart.html',
  styleUrls: ['./my-task-deadline-bar-chart.css']
})
export class MyTaskDeadlineBarChartComponent implements OnInit {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  public barChartLabels: string[] = ['Overdue', 'Today', 'Next 3 Days', 'This Week'];
  public barChartData: ChartData<'bar'> = {
    labels: this.barChartLabels,
    datasets: [
      {
        data: [0, 0, 0, 0],
        label: 'Tasks',
        backgroundColor: '#66BB6A',
        maxBarThickness: 40
      }
    ]
  };
  public barChartType: ChartType = 'bar';

  public barChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      x: {
        ticks: { font: { size: 12 } }
      },
      y: {
        beginAtZero: true,
        ticks: { stepSize: 1 }
      }
    },
    plugins: {
      legend: { labels: { font: { size: 12 } } },
      tooltip: { enabled: true }
    }
  };

  constructor(private taskService: TaskService, private authService: AuthService) {}

  ngOnInit(): void {
    const userId = this.authService.getUser()?.id;

    if (!userId) return;

    this.taskService.getAllTasks(1, 1000).subscribe({
      next: (res) => {
        const now = new Date();
        const tasks = (res.data || []).filter((t: any) => t.assignedToId === userId);

        let overdue = 0;
        let today = 0;
        let next3Days = 0;
        let thisWeek = 0;

        tasks.forEach((task: any) => {
          const due = new Date(task.dueDate);
          const daysDiff = Math.ceil((due.getTime() - now.getTime()) / (1000 * 3600 * 24));

          if (due < now) overdue++;
          else if (daysDiff === 0) today++;
          else if (daysDiff <= 3) next3Days++;
          else if (daysDiff <= 7) thisWeek++;
        });

        this.barChartData.datasets[0].data = [overdue, today, next3Days, thisWeek];
        this.chart?.update();
      },
      error: (err) => {
        console.error('Failed to load task deadline chart', err);
      }
    });
  }

  hasBarChartData(): boolean {
    const data = this.barChartData.datasets[0].data;
    return Array.isArray(data) && data.some(d => typeof d === 'number' && d > 0);
  }
}
