import { Component } from '@angular/core';
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
} from 'ng-apexcharts';
import { UserService } from '../../service/user.service';
import dayjs from 'dayjs';

@Component({
  selector: 'app-bar-chart2',
  standalone: true,
  imports: [NgApexchartsModule],
  templateUrl: './bar-chart2.html',
  styleUrl: './bar-chart2.css',
})
export class BarChart2 {
  public series!: ApexAxisChartSeries;
  public chart!: ApexChart;
  public dataLabels!: ApexDataLabels;
  public markers!: ApexMarkers;
  public title!: ApexTitleSubtitle;
  public fill!: ApexFill;
  public yaxis!: ApexYAxis;
  public xaxis!: ApexXAxis;
  public tooltip!: ApexTooltip;

  constructor(private userService: UserService) {
    this.fetchAndPrepareChartData();
  }

  fetchAndPrepareChartData() {
    this.userService.getLogsAnalysis().subscribe((res) => {
      const logs = res?.data?.$values || [];
      const today = dayjs();
      const last7Days = new Map<string, number>();

      for (let i = 0; i < 7; i++) {
        const day = today.subtract(6 - i, 'day').format('YYYY-MM-DD');
        last7Days.set(day, 0);
      }

      logs.forEach((log: any) => {
        const logDate = dayjs(log.date).format('YYYY-MM-DD');
        if (last7Days.has(logDate)) {
          last7Days.set(logDate, last7Days.get(logDate)! + 1);
        }
      });

      const dateSeries: [number, number][] = Array.from(
        last7Days.entries()
      ).map(([date, count]) => [new Date(date).getTime(), count]);

      this.initChartData(dateSeries);
    });
  }

  initChartData(dates: [number, number][]): void {
    const today = new Date();
      const start = new Date(today);
    this.series = [
      {
        name: 'Submitted Logs',
        data: dates,
      },
    ];

    this.chart = {
      type: 'area',
      stacked: false,
      height: 450,
      zoom: {
        enabled: false,
      },
      toolbar: {
        show: false,
      },
    };

    this.dataLabels = {
      enabled: false,
    };

    this.markers = {
      size: 4,
    };

    this.fill = {
      type: 'gradient',
      gradient: {
        shadeIntensity: 1,
        inverseColors: false,
        opacityFrom: 0.5,
        opacityTo: 0,
        stops: [0, 90, 100],
      },
    };

    this.yaxis = {
      labels: {
        formatter: function (val) {
          return val.toFixed(0);
        },
      },
      title: {
        text: 'Logs Count',
      },
    };

    this.xaxis = {
      type: 'datetime',
      labels: {
        datetimeFormatter: {
          day: 'dd MMM',
        },
      },
    };

    this.tooltip = {
      shared: false,
      x: {
        format: 'dd MMM yyyy',
      },
      y: {
        formatter: function (val) {
          return val.toFixed(0);
        },
      },
    };
  }
}
