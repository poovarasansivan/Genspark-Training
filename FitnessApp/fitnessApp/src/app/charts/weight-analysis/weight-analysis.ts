import { Component, ViewChild } from '@angular/core';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexTitleSubtitle,
  ApexDataLabels,
  ApexFill,
  ApexMarkers,
  ApexYAxis,
  ApexXAxis,
  ApexTooltip,
  NgApexchartsModule,
  ChartComponent,
} from 'ng-apexcharts';
import { UserStatsService } from '../../service/userstats.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-weight-analysis',
  standalone: true,
  imports: [NgApexchartsModule, CommonModule],
  templateUrl: './weight-analysis.html',
  styleUrl: './weight-analysis.css',
})
export class WeightAnalysis {
   @ViewChild('chart') chartComponent?: ChartComponent;
  public series: ApexAxisChartSeries = [
    {
      name: 'Body Weight (kg)',
      data: [],
    },
  ];

  chartReady: boolean = false;

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
    title: { text: 'Body Weight (kg)' },
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
            const Weight = progressList.map((entry: any) => entry.weight);
            const weekLabels = progressList.map(
              (_: any, index: number) => `Week ${index + 1}`
            );
            this.series = [
              {
                name: 'Body Weight (kg)',
                data: Weight,
              },
            ];
            this.xaxis = {
              categories: weekLabels,
              title: { text: 'Weeks' },
            };
            this.chartReady = true;
          },

          error: (error) => {
            console.error('Error fetching progress stats:', error);
          },
        });
    }
    
  }
  
}


