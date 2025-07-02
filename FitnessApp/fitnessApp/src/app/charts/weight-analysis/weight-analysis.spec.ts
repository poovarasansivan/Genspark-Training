import { ComponentFixture, TestBed } from '@angular/core/testing';
import { WeightAnalysis } from './weight-analysis';
import { UserStatsService } from '../../service/userstats.service';
import { of, throwError } from 'rxjs';
import { NgApexchartsModule } from 'ng-apexcharts';
import { CommonModule } from '@angular/common';

describe('WeightAnalysis Component', () => {
  let component: WeightAnalysis;
  let fixture: ComponentFixture<WeightAnalysis>;
  let userStatsServiceSpy: jasmine.SpyObj<UserStatsService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('UserStatsService', ['getProgressStats']);

    await TestBed.configureTestingModule({
      imports: [WeightAnalysis, NgApexchartsModule, CommonModule],
      providers: [{ provide: UserStatsService, useValue: spy }],
    }).compileComponents();

    fixture = TestBed.createComponent(WeightAnalysis);
    component = fixture.componentInstance;
    userStatsServiceSpy = TestBed.inject(UserStatsService) as jasmine.SpyObj<UserStatsService>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with default chart config', () => {
    expect(component.series).toEqual([{ name: 'Body Weight (kg)', data: [] }]);
    expect(component.chart.type).toBe('area');
    expect(component.chartReady).toBeFalse();
    expect(component.xaxis.title?.text).toBe('Weeks');
    expect(component.yaxis.title?.text).toBe('Body Weight (kg)');
  });

  describe('ngOnInit()', () => {
    beforeEach(() => {
      spyOn(localStorage, 'getItem').and.callFake((key: string) => {
        if (key === 'userId') return 'test-user-id';
        if (key === 'workOutPlanId') return 'test-plan-id';
        return null;
      });
    });

    it('should fetch progress stats and update series and xaxis on success (array data)', () => {
      const mockResponse = {
        data: {
          $values: [
            { weight: 70 },
            { weight: 68 },
            { weight: 67 }
          ],
        },
      };

      userStatsServiceSpy.getProgressStats.and.returnValue(of(mockResponse));

      component.ngOnInit();

      expect(userStatsServiceSpy.getProgressStats).toHaveBeenCalledWith('test-user-id', 'test-plan-id');
      expect(component.series).toEqual([
        { name: 'Body Weight (kg)', data: [70, 68, 67] },
      ]);
      expect(component.xaxis.categories).toEqual(['Week 1', 'Week 2', 'Week 3']);
      expect(component.chartReady).toBeTrue();
    });

    it('should handle single object response data correctly', () => {
      const mockResponse = {
        data: { weight: 72 },
      };

      userStatsServiceSpy.getProgressStats.and.returnValue(of(mockResponse));

      component.ngOnInit();

      expect(component.series).toEqual([
        { name: 'Body Weight (kg)', data: [72] },
      ]);
      expect(component.xaxis.categories).toEqual(['Week 1']);
      expect(component.chartReady).toBeTrue();
    });

    it('should log error if API call fails', () => {
      const consoleSpy = spyOn(console, 'error');
      userStatsServiceSpy.getProgressStats.and.returnValue(throwError(() => new Error('API Error')));

      component.ngOnInit();

      expect(consoleSpy).toHaveBeenCalledWith('Error fetching progress stats:', jasmine.any(Error));
      expect(component.chartReady).toBeFalse();
    });

    it('should not call service if userId or workoutPlanId is missing', () => {
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
