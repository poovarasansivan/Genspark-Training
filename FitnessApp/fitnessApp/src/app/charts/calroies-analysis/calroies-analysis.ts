import { Component, ViewChild, OnInit } from '@angular/core';
import { NgApexchartsModule } from 'ng-apexcharts';
import {
  ApexAxisChartSeries,
  ApexChart,
  ChartComponent,
  ApexDataLabels,
  ApexPlotOptions,
  ApexYAxis,
  ApexLegend,
  ApexStroke,
  ApexXAxis,
  ApexFill,
  ApexTooltip,
} from 'ng-apexcharts';
import { UserStatsService } from '../../service/userstats.service';
import { TokenService } from '../../service/token.service';
import { CommonModule } from '@angular/common';

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  yaxis: ApexYAxis;
  xaxis: ApexXAxis;
  fill: ApexFill;
  tooltip: ApexTooltip;
  stroke: ApexStroke;
  legend: ApexLegend;
  colors?: string[];
};

@Component({
  selector: 'app-calroies-analysis',
  standalone: true,
  imports: [NgApexchartsModule, CommonModule],
  templateUrl: './calroies-analysis.html',
  styleUrl: './calroies-analysis.css',
})
export class CalroiesAnalysis implements OnInit {

  @ViewChild('chart') chart?: ChartComponent;

  chartReady: boolean = false;
  public chartOptions: ChartOptions = {
    series: [{ name: 'Calories Burned', data: [] }],
    chart: {
      type: 'bar',
      height: 450,
      toolbar: { show: false },
    },
    colors: ['oklch(70.7% 0.165 254.624)'],
    plotOptions: {
      bar: {
        horizontal: false,
        columnWidth: '50%',
        borderRadius: 5,
      },
    },
    dataLabels: {
      enabled: true,
      formatter: (val: number) => `${val} cal`,
    },
    stroke: {
      show: true,
      width: 2,
      colors: ['transparent'],
    },
    xaxis: {
      categories: [],
      title: { text: 'Workout Days' },
    },
    yaxis: {
      title: { text: 'Calories Burned' },
    },
    fill: {
      opacity: 1,
    },
    tooltip: {
      y: {
        formatter: (val: number, opts) => {
          const index = opts.dataPointIndex;
          return `Calories: ${val}`;
        },
      },
    },
    legend: {
      position: 'top',
      horizontalAlign: 'center',
    },
  };

  workoutPlanId: string | null = null;
  clientId: string | null = null;

  constructor(private userStatsService: UserStatsService) {}

  ngOnInit(): void {
    this.clientId = localStorage.getItem('userId');
    this.workoutPlanId = localStorage.getItem('workOutPlanId');
    if (this.clientId || this.workoutPlanId) {
      this.userStatsService
        .getWorkOutStats(this.clientId, this.workoutPlanId)
        .subscribe({
          next: (response) => {
            const logs = response.data?.$values || [];

            const calories = logs.map((log: any) => log.caloriesBurned);
            const durations = logs.map((log: any) =>
              this.convertDurationToMinutes(log.duration)
            );
            const categories = logs.map(
              (_: any, index: number) => `Day ${index + 1}`
            );

            this.setupChart(calories, durations, categories);
            this.chartReady = true;
          },
          error: (err) => {
            console.error('Error fetching logs:', err);
          },
        });
    } else {
      console.warn('Missing clientId or workoutPlanId in localStorage');
    }
  }

  convertDurationToMinutes(duration: string): number {
    const parts = duration.split(':').map(Number);
    return parts[0] * 60 + parts[1]; // HH:mm:ss to total minutes
  }

  setupChart(
    calories: number[],
    durations: number[],
    categories: string[]
  ): void {
    this.chartOptions = {
      ...this.chartOptions,
      series: [{ name: 'Calories Burned', data: calories }],
      xaxis: {
        categories,
        title: { text: 'Workout Days' },
      },
      tooltip: {
        y: {
          formatter: (val: number, opts) => {
            const index = opts.dataPointIndex;
            const duration = durations[index];
            return `${val} cal in ${duration} mins`;
          },
        },
      },
    };

    // ✅ Force ApexChart to re-render
    setTimeout(() => {
      this.chart?.updateOptions(this.chartOptions, true, true);
    });
  }
}
