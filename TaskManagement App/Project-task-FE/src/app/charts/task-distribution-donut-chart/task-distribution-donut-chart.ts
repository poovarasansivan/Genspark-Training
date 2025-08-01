import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartConfiguration, ChartType, ChartData } from 'chart.js';
import { NgChartsModule, BaseChartDirective } from 'ng2-charts';
import { TaskService } from '../../manager/create-task/task.service';

@Component({
  selector: 'app-task-distribution-donut-chart',
  standalone: true,
  imports: [CommonModule, NgChartsModule],
  templateUrl: './task-distribution-donut-chart.html',
  styleUrls: ['./task-distribution-donut-chart.css'],
})
export class TaskDistributionDonutChartComponent implements OnInit {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  taskCounts = {
    Draft: 0,
    ToDo: 0,
    InProgress: 0,
    Done: 0,
  };

  public doughnutChartLabels: string[] = [
    'Draft',
    'To Do',
    'In Progress',
    'Done',
  ];

  public doughnutChartData: ChartData<'doughnut'> = {
    labels: this.doughnutChartLabels,
    datasets: [
      {
        data: [0, 0, 0, 0],
        backgroundColor: ['#F88379', '#3498db', '#f39c12', '#2ecc71'],
      },
    ],
  };

  public doughnutChartType: ChartType = 'doughnut';

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.chart?.update();

    this.taskService.getAllTasks(1, 1000).subscribe({
      next: (res) => {
        const tasks = res.data;
        this.taskCounts.Draft = tasks.filter((t: any) => t.status === 'Draft').length;
        this.taskCounts.ToDo = tasks.filter(
          (t: any) => t.status === 'ToDo'
        ).length;
        this.taskCounts.InProgress = tasks.filter(
          (t: any) => t.status === 'InProgress'
        ).length;
        this.taskCounts.Done = tasks.filter(
          (t: any) => t.status === 'Done'
        ).length;

        this.doughnutChartData.datasets[0].data = [
          this.taskCounts.Draft,
          this.taskCounts.ToDo,
          this.taskCounts.InProgress,
          this.taskCounts.Done,
        ];
        this.chart?.update();
      },
      error: (err) => {
        console.error('Failed to load tasks for chart', err);
      },
    });
  }

  hasChartData(): boolean {
    return (
      this.doughnutChartData &&
      this.doughnutChartData.datasets &&
      this.doughnutChartData.datasets.length > 0 &&
      this.doughnutChartData.datasets[0].data.some((value) => value > 0)
    );
  }
}
