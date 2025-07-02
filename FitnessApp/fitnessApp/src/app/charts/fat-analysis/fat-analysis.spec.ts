import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FatAnalysis } from './fat-analysis';
import { UserStatsService } from '../../service/userstats.service';
import { of, throwError } from 'rxjs';
import { NgApexchartsModule } from 'ng-apexcharts';
import { CommonModule } from '@angular/common';

describe('FatAnalysis Component', () => {
  let component: FatAnalysis;
  let fixture: ComponentFixture<FatAnalysis>;
  let userStatsServiceSpy: jasmine.SpyObj<UserStatsService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('UserStatsService', ['getProgressStats']);

    await TestBed.configureTestingModule({
      imports: [FatAnalysis, NgApexchartsModule, CommonModule],
      providers: [{ provide: UserStatsService, useValue: spy }],
    }).compileComponents();

    fixture = TestBed.createComponent(FatAnalysis);
    component = fixture.componentInstance;
    userStatsServiceSpy = TestBed.inject(UserStatsService) as jasmine.SpyObj<UserStatsService>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with default chart config', () => {
    expect(component.series).toEqual([
      { name: 'Body Fat (%)', data: [] },
    ]);
    expect(component.chart.type).toBe('area');
    expect(component.chartReady).toBeFalse();
    expect(component.xaxis.title?.text).toBe('Weeks');
    expect(component.yaxis.title?.text).toBe('Body Fat (%)');
  });

  describe('ngOnInit()', () => {
    beforeEach(() => {
      spyOn(localStorage, 'getItem').and.callFake((key: string) => {
        if (key === 'userId') return 'test-user-id';
        if (key === 'workOutPlanId') return 'test-plan-id';
        return null;
      });
    });

    it('should fetch fat data and update chart with array response', () => {
      const mockResponse = {
        data: {
          $values: [
            { bodyFatPercentage: 21.5 },
            { bodyFatPercentage: 20.9 },
            { bodyFatPercentage: 20.2 },
          ],
        },
      };

      userStatsServiceSpy.getProgressStats.and.returnValue(of(mockResponse));

      component.ngOnInit();

      expect(userStatsServiceSpy.getProgressStats).toHaveBeenCalledWith('test-user-id', 'test-plan-id');
      expect(component.series).toEqual([
        { name: 'Body Fat (%)', data: [21.5, 20.9, 20.2] },
      ]);
      expect(component.xaxis.categories).toEqual(['Week 1', 'Week 2', 'Week 3']);
      expect(component.chartReady).toBeTrue();
    });

    it('should handle single object response data correctly', () => {
      const mockResponse = {
        data: { bodyFatPercentage: 19.8 },
      };

      userStatsServiceSpy.getProgressStats.and.returnValue(of(mockResponse));

      component.ngOnInit();

      expect(component.series).toEqual([
        { name: 'Body Fat (%)', data: [19.8] },
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
        'Error fetching fat data:',
        jasmine.any(Error)
      );
      expect(component.chartReady).toBeFalse();
    });

    it('should not call getProgressStats if IDs are missing', () => {
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
