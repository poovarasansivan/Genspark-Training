import { ComponentFixture, TestBed } from '@angular/core/testing';
import { WaterAnalysis } from './water-analysis';
import { UserStatsService } from '../../service/userstats.service';
import { of, throwError } from 'rxjs';
import { NgApexchartsModule } from 'ng-apexcharts';
import { CommonModule } from '@angular/common';

describe('WaterAnalysis Component', () => {
  let component: WaterAnalysis;
  let fixture: ComponentFixture<WaterAnalysis>;
  let userStatsServiceSpy: jasmine.SpyObj<UserStatsService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('UserStatsService', ['getProgressStats']);

    await TestBed.configureTestingModule({
      imports: [WaterAnalysis, NgApexchartsModule, CommonModule],
      providers: [{ provide: UserStatsService, useValue: spy }],
    }).compileComponents();

    fixture = TestBed.createComponent(WaterAnalysis);
    component = fixture.componentInstance;
    userStatsServiceSpy = TestBed.inject(UserStatsService) as jasmine.SpyObj<UserStatsService>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with default chart config', () => {
    expect(component.series).toEqual([{ name: 'Body Water Percentage (%)', data: [] }]);
    expect(component.chart.type).toBe('area');
    expect(component.chartReady).toBeFalse();
    expect(component.xaxis.title?.text).toBe('Weeks');
    expect(component.yaxis.title?.text).toBe('Body Water Percentage (%)');
  });

  describe('ngOnInit()', () => {
    beforeEach(() => {
      spyOn(localStorage, 'getItem').and.callFake((key: string) => {
        if (key === 'userId') return 'test-user-id';
        if (key === 'workOutPlanId') return 'test-plan-id';
        return null;
      });
    });

    it('should fetch progress stats and update chart with array data', () => {
      const mockResponse = {
        data: {
          $values: [
            { waterPercentage: 55.2 },
            { waterPercentage: 56.1 },
            { waterPercentage: 57.0 },
          ],
        },
      };

      userStatsServiceSpy.getProgressStats.and.returnValue(of(mockResponse));

      component.ngOnInit();

      expect(userStatsServiceSpy.getProgressStats).toHaveBeenCalledWith('test-user-id', 'test-plan-id');
      expect(component.series).toEqual([
        { name: 'Body Water Percentage (%)', data: [55.2, 56.1, 57.0] },
      ]);
      expect(component.xaxis.categories).toEqual(['Week 1', 'Week 2', 'Week 3']);
      expect(component.chartReady).toBeTrue();
    });

    it('should handle single object response data correctly', () => {
      const mockResponse = {
        data: { waterPercentage: 60.5 },
      };

      userStatsServiceSpy.getProgressStats.and.returnValue(of(mockResponse));

      component.ngOnInit();

      expect(component.series).toEqual([
        { name: 'Body Water Percentage (%)', data: [60.5] },
      ]);
      expect(component.xaxis.categories).toEqual(['Week 1']);
      expect(component.chartReady).toBeTrue();
    });

    it('should log error if API call fails', () => {
      const consoleSpy = spyOn(console, 'error');
      userStatsServiceSpy.getProgressStats.and.returnValue(
        throwError(() => new Error('API Error'))
      );

      component.ngOnInit();

      expect(consoleSpy).toHaveBeenCalledWith(
        'Error fetching progress stats:',
        jasmine.any(Error)
      );
      expect(component.chartReady).toBeFalse();
    });

    it('should not call getProgressStats if userId or workoutPlanId is missing', () => {
      (localStorage.getItem as jasmine.Spy).and.callFake((key: string) => {
        if (key === 'userId') return null;
        if (key === 'workOutPlanId') return 'test-plan-id';
        return null;
      });

      component.ngOnInit();

      expect(userStatsServiceSpy.getProgressStats).not.toHaveBeenCalled();
      expect(component.chartReady).toBeFalse();
    });
  });
});
