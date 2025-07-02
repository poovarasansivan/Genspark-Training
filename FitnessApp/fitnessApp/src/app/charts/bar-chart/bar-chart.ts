import { Component, ViewChild } from '@angular/core';
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
import { UserService } from '../../service/user.service';

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
  selector: 'app-bar-chart',
  standalone: true,
  imports: [NgApexchartsModule],
  templateUrl: './bar-chart.html',
  styleUrl: './bar-chart.css',
})
export class BarChart {
  @ViewChild('chart')
  chart!: ChartComponent;
  public chartOptions: ChartOptions;

  constructor(private userService: UserService) {
    this.chartOptions = {
      series: [],
      chart: { type: 'bar', height: 450 },
      colors: [
        'oklch(76.5% 0.177 163.223)',
        'oklch(70.7% 0.165 254.624)',
        'oklch(80.8% 0.114 19.571)',
      ],
      plotOptions: {
        bar: { horizontal: false, columnWidth: '55%' },
      },
      dataLabels: { enabled: false },
      stroke: {
        show: true,
        width: 2,
        colors: ['transparent'],
      },
      xaxis: { categories: [] },
      yaxis: { title: { text: 'Users' } },
      fill: { opacity: 1 },
      tooltip: {
        y: {
          formatter: function (val) {
            return val.toString();
          },
        },
      },
      legend: {
        position: 'top',
        horizontalAlign: 'center',
      },
    };

    this.loadChartData();
  }

  loadChartData() {
    this.userService.getPlanAnalysis().subscribe((response) => {
      const plans = response.data.$values;
      const planMap = new Map<
        string,
        {
          name: string;
          userCount: number;
          coachSet: Set<string>;
          completedCount: number;
        }
      >();

      plans.forEach(
        (plan: {
          workOutPlanId: any;
          workOutPlanName: any;
          coachId: string;
          isCompleted: string;
        }) => {
          const id = plan.workOutPlanId;
          if (!planMap.has(id)) {
            planMap.set(id, {
              name: plan.workOutPlanName,
              userCount: 0,
              coachSet: new Set<string>(),
              completedCount: 0,
            });
          }

          const entry = planMap.get(id)!;
          entry.userCount += 1;
          entry.coachSet.add(plan.coachId);
          if (plan.isCompleted === 'Completed') {
            entry.completedCount += 1;
          }
        }
      );

      const categories: string[] = [];
      const userCounts: number[] = [];
      const coachCounts: number[] = [];
      const completedCounts: number[] = [];

      for (let entry of planMap.values()) {
        categories.push(entry.name);
        userCounts.push(entry.userCount);
        coachCounts.push(entry.coachSet.size);
        completedCounts.push(entry.completedCount);
      }

      this.chartOptions.series = [
        { name: 'Users', data: userCounts },
        { name: 'Coaches', data: coachCounts },
        { name: 'Completed Users', data: completedCounts },
      ];
      this.chartOptions.xaxis = {
        categories: categories,
      };
    });
  }
}
