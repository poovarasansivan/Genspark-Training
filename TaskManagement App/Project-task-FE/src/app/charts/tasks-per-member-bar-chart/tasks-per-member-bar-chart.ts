import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartData, ChartType, ChartOptions } from 'chart.js';
import { NgChartsModule, BaseChartDirective } from 'ng2-charts';
import { TaskService } from '../../manager/create-task/task.service';

@Component({
  selector: 'app-tasks-per-member-bar-chart',
  standalone: true,
  imports: [CommonModule, NgChartsModule],
  templateUrl: './tasks-per-member-bar-chart.html',
  styleUrls: ['./tasks-per-member-bar-chart.css']
})
export class TasksPerMemberBarChartComponent implements OnInit {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  public barChartLabels: string[] = [];
  public barChartData: ChartData<'bar'> = {
    labels: [],
    datasets: [
      {
        data: [],
        label: 'Assigned Tasks',
        backgroundColor: '#42A5F5',
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
        ticks: {
          font: { size: 10 },
          maxRotation: 45,
          minRotation: 20
        }
      },
      y: {
        beginAtZero: true,
        ticks: {
          stepSize: 1
        }
      }
    },
    plugins: {
      legend: {
        labels: {
          font: { size: 12 }
        }
      },
      tooltip: {
        enabled: true
      }
    }
  };

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.taskService.getAllTasks(1, 1000).subscribe({
      next: (res) => {
        const tasks = res.data || [];
        const counts: { [user: string]: number } = {};
        const names: { [id: string]: string } = {};

        tasks.forEach((task: any) => {
          if(!task.assignedToId) return; // Skip tasks without assigned user
          const userId = task.assignedToId;
          const userName = task.assignedToName || 'Unknown';

          names[userId] = userName;
          counts[userName] = (counts[userName] || 0) + 1;
        });

        this.barChartData.labels = Object.keys(counts);
        this.barChartData.datasets[0].data = Object.values(counts);
        this.chart?.update();
      },
      error: (err) => {
        console.error('Error loading task data for bar chart', err);
      }
    });
  }
  hasBarChartData(): boolean {
  const dataset = this.barChartData.datasets[0];
  return Array.isArray(dataset?.data) &&
    dataset.data.some(value =>
      typeof value === 'number' && value > 0
    );
}
}
