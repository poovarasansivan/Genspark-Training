import { Component, OnInit, ViewChild } from '@angular/core';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexFill,
  ApexYAxis,
  ApexXAxis,
  ApexTooltip,
  ApexLegend,
  ChartComponent,
  NgApexchartsModule,
} from 'ng-apexcharts';
import { UserStatsService } from '../../service/userstats.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-fat-analysis',
  standalone: true,
  imports: [NgApexchartsModule, CommonModule],
  templateUrl: './fat-analysis.html',
  styleUrl: './fat-analysis.css',
})
export class FatAnalysis implements OnInit {
  @ViewChild('chart') chartComponent?: ChartComponent;

  chartReady: boolean = false;

  public series: ApexAxisChartSeries = [
    {
      name: 'Body Fat (%)',
      data: [],
    },
  ];

  public chart: ApexChart = {
    type: 'area',
    stacked: false,
    height: 450,
    zoom: { enabled: false },
    toolbar: { show: false },
  };

  public xaxis: ApexXAxis = {
    categories: [],
    title: { text: 'Weeks' },
  };

  public yaxis: ApexYAxis = {
    title: { text: 'Body Fat (%)' },
    labels: {
      formatter: (val: number) => val.toFixed(1),
    },
  };

  public dataLabels: ApexDataLabels = {
    enabled: true,
  };

  public fill: ApexFill = {
    type: 'gradient',
    gradient: {
      shadeIntensity: 1,
      inverseColors: false,
      opacityFrom: 0.5,
      opacityTo: 0,
      stops: [0, 90, 100],
    },
  };

  public tooltip: ApexTooltip = {
    y: {
      formatter: (val: number) => `${val.toFixed(1)} %`,
    },
  };

  public legend: ApexLegend = {
    position: 'top',
    horizontalAlign: 'center',
  };

  userId: string | null = null;
  workoutPlanId: string | null = null;

  constructor(private userStatsService: UserStatsService) {}

  ngOnInit(): void {
    this.userId = localStorage.getItem('userId');
    this.workoutPlanId = localStorage.getItem('workOutPlanId');
    if (this.userId && this.workoutPlanId) {
      this.userStatsService
        .getProgressStats(this.userId, this.workoutPlanId)
        .subscribe({
          next: (response) => {
            const progressList = response.data?.$values || [response.data];
            const fatPercentages = progressList.map(
              (entry: any) => entry.bodyFatPercentage
            );
            const weekLabels = progressList.map(
              (_: any, index: number) => `Week ${index + 1}`
            );

            this.series = [
              {
                name: 'Body Fat (%)',
                data: fatPercentages,
              },
            ];

            this.xaxis = {
              categories: weekLabels,
              title: { text: 'Weeks' },
            };
            this.chartReady = true;
          },
          error: (err) => {
            console.error('Error fetching fat data:', err);
          },
        });
    }
  }
}
