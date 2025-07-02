import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CalroiesAnalysis } from './calroies-analysis';
import { UserStatsService } from '../../service/userstats.service';
import { of, throwError } from 'rxjs';
import { NgApexchartsModule } from 'ng-apexcharts';
import { CommonModule } from '@angular/common';

describe('CalroiesAnalysis Component', () => {
  let component: CalroiesAnalysis;
  let fixture: ComponentFixture<CalroiesAnalysis>;
  let userStatsSpy: jasmine.SpyObj<UserStatsService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('UserStatsService', ['getWorkOutStats']);

    await TestBed.configureTestingModule({
      imports: [CalroiesAnalysis, NgApexchartsModule, CommonModule],
      providers: [{ provide: UserStatsService, useValue: spy }],
    }).compileComponents();

    fixture = TestBed.createComponent(CalroiesAnalysis);
    component = fixture.componentInstance;
    userStatsSpy = TestBed.inject(UserStatsService) as jasmine.SpyObj<UserStatsService>;
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  it('should have default chart options', () => {
    expect(component.chartOptions.chart?.type).toBe('bar');
    expect(component.chartOptions.series).toEqual([{ name: 'Calories Burned', data: [] }]);
    expect(component.chartOptions.xaxis?.title?.text).toBe('Workout Days');
    expect(component.chartOptions.yaxis?.title?.text).toBe('Calories Burned');
    expect(component.chartReady).toBeFalse();
  });

  describe('ngOnInit()', () => {
    beforeEach(() => {
      spyOn(localStorage, 'getItem').and.callFake((key: string) => {
        if (key === 'userId') return 'mock-user-id';
        if (key === 'workOutPlanId') return 'mock-plan-id';
        return null;
      });
    });

    it('should fetch workout stats and update chart', () => {
      const mockData = {
        data: {
          $values: [
            { caloriesBurned: 300, duration: '01:15:00' },
            { caloriesBurned: 450, duration: '00:45:00' },
          ],
        },
      };

      userStatsSpy.getWorkOutStats.and.returnValue(of(mockData));

      const setupSpy = spyOn(component, 'setupChart').and.callThrough();

      component.ngOnInit();

      expect(userStatsSpy.getWorkOutStats).toHaveBeenCalledWith('mock-user-id', 'mock-plan-id');
      expect(setupSpy).toHaveBeenCalledWith(
        [300, 450],
        [75, 45],
        ['Day 1', 'Day 2']
      );
      expect(component.chartReady).toBeTrue();
    });

    it('should handle API error', () => {
      const errorSpy = spyOn(console, 'error');
      userStatsSpy.getWorkOutStats.and.returnValue(throwError(() => new Error('API Error')));

      component.ngOnInit();

      expect(errorSpy).toHaveBeenCalledWith('Error fetching logs:', jasmine.any(Error));
      expect(component.chartReady).toBeFalse();
    });

    it('should warn if IDs are missing', () => {
      (localStorage.getItem as jasmine.Spy).and.callFake(() => null);

      const warnSpy = spyOn(console, 'warn');

      component.ngOnInit();

      expect(warnSpy).toHaveBeenCalledWith('Missing clientId or workoutPlanId in localStorage');
      expect(userStatsSpy.getWorkOutStats).not.toHaveBeenCalled();
    });
  });

  it('convertDurationToMinutes should convert HH:mm:ss to minutes', () => {
    expect(component.convertDurationToMinutes('01:30:00')).toBe(90);
    expect(component.convertDurationToMinutes('00:45:00')).toBe(45);
    expect(component.convertDurationToMinutes('02:00:00')).toBe(120);
  });
});
