import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BarChart2 } from './bar-chart2';
import { UserService } from '../../service/user.service';
import { of } from 'rxjs';
import dayjs from 'dayjs';

describe('BarChart2 Component', () => {
  let component: BarChart2;
  let fixture: ComponentFixture<BarChart2>;
  let userServiceSpy: jasmine.SpyObj<UserService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('UserService', ['getLogsAnalysis']);
    spy.getLogsAnalysis.and.returnValue(of({ data: { $values: [] } }));

    await TestBed.configureTestingModule({
      imports: [BarChart2],
      providers: [{ provide: UserService, useValue: spy }],
    }).compileComponents();

    fixture = TestBed.createComponent(BarChart2);
    component = fixture.componentInstance;
    userServiceSpy = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch logs and initialize chart data correctly', () => {
    const logs = Array.from({ length: 7 }, (_, i) => ({
      date: dayjs().subtract(i, 'day').toISOString(),
    }));

    userServiceSpy.getLogsAnalysis.and.returnValue(
      of({ data: { $values: logs } })
    );

    const initSpy = spyOn(component, 'initChartData').and.callThrough();

    component.fetchAndPrepareChartData();

    expect(userServiceSpy.getLogsAnalysis).toHaveBeenCalled();

    expect(initSpy).toHaveBeenCalled();
    const [arg] = initSpy.calls.mostRecent().args;
    expect(arg.length).toBe(7);

    expect(component.series).toBeDefined();
    expect(component.chart).toBeDefined();
    expect(component.xaxis.type).toBe('datetime');
    expect(component.yaxis.title?.text).toBe('Logs Count');
  });

  it('should initialize chart properties in initChartData()', () => {
    const dates: [number, number][] = [
      [new Date('2024-01-01').getTime(), 5],
      [new Date('2024-01-02').getTime(), 3],
    ];

    component.initChartData(dates);

    expect(component.series).toEqual([{ name: 'Submitted Logs', data: dates }]);
    expect(component.chart?.type).toBe('area');
    expect(component.fill?.type).toBe('gradient');
    expect(component.dataLabels?.enabled).toBeFalse();
    expect(component.markers?.size).toBe(4);
    expect(component.tooltip?.x?.format).toBe('dd MMM yyyy');
  });

  it('should handle empty logs gracefully', () => {
    userServiceSpy.getLogsAnalysis.and.returnValue(
      of({ data: { $values: [] } })
    );

    const initSpy = spyOn(component, 'initChartData').and.callThrough();

    component.fetchAndPrepareChartData();

    expect(initSpy).toHaveBeenCalled();

    const [arg] = initSpy.calls.mostRecent().args;
    // Should still have 7 days initialized, all zeros
    expect(arg.length).toBe(7);
    arg.forEach(([_, count]) => expect(count).toBe(0));
  });

  it('should handle missing $values gracefully', () => {
    userServiceSpy.getLogsAnalysis.and.returnValue(of({ data: [] }));

    const initSpy = spyOn(component, 'initChartData').and.callThrough();

    component.fetchAndPrepareChartData();

    expect(initSpy).toHaveBeenCalled();
    const [arg] = initSpy.calls.mostRecent().args;
    expect(arg.length).toBe(7);
  });
});
