import { ComponentFixture, TestBed } from '@angular/core/testing';
import { WorkoutLogs } from './workout-logs';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { of, throwError } from 'rxjs';
import { WorkOutLogService } from '../service/workout-log.service';
import { WorkOutPlanService } from '../service/workout-plan.service';
import { TokenService } from '../service/token.service';
import { Popup } from '../popup/popup';

describe('WorkoutLogs', () => {
  let component: WorkoutLogs;
  let fixture: ComponentFixture<WorkoutLogs>;

  let mockWorkOutLogService: jasmine.SpyObj<WorkOutLogService>;
  let mockWorkOutPlanService: jasmine.SpyObj<WorkOutPlanService>;
  let mockTokenService: jasmine.SpyObj<TokenService>;

  beforeEach(async () => {
    mockWorkOutLogService = jasmine.createSpyObj('WorkOutLogService', [
      'getAllWorkOutLogsWithPagination',
      'getWorkOutByCoachId',
      'getWorkOutByClientId',
      'addNewWorkOutLog',
    ]);
    mockWorkOutPlanService = jasmine.createSpyObj('WorkOutPlanService', [
      'getWorkOutPlans',
    ]);
    mockTokenService = jasmine.createSpyObj('TokenService', [
      'getRole',
      'getUserId',
      'getUsername',
    ]);

    mockWorkOutLogService.getAllWorkOutLogsWithPagination.and.returnValue(
      of({ data: [], totalCount: 0 })
    );
    mockWorkOutLogService.getWorkOutByCoachId.and.returnValue(of([]));
    mockWorkOutLogService.getWorkOutByClientId.and.returnValue(of([]));
    mockWorkOutLogService.addNewWorkOutLog.and.returnValue(of({}));
    mockWorkOutPlanService.getWorkOutPlans.and.returnValue(of([]));

    mockTokenService.getRole.and.returnValue('Admin');
    mockTokenService.getUserId.and.returnValue('user1');
    mockTokenService.getUsername.and.returnValue('John Doe');

    await TestBed.configureTestingModule({
      imports: [WorkoutLogs, CommonModule, FormsModule, ReactiveFormsModule],
      providers: [
        { provide: WorkOutLogService, useValue: mockWorkOutLogService },
        { provide: WorkOutPlanService, useValue: mockWorkOutPlanService },
        { provide: TokenService, useValue: mockTokenService },
        { provide: Popup, useValue: { display: jasmine.createSpy('display') } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(WorkoutLogs);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should load logs for Admin', () => {
      component.ngOnInit();
      expect(mockWorkOutLogService.getAllWorkOutLogsWithPagination).toHaveBeenCalled();
    });

    it('should load logs for Coach', () => {
      mockTokenService.getRole.and.returnValue('Coach');
      component.ngOnInit();
      expect(mockWorkOutLogService.getWorkOutByCoachId).toHaveBeenCalled();
    });

    it('should load logs for User', () => {
      mockTokenService.getRole.and.returnValue('User');
      component.ngOnInit();
      expect(mockWorkOutLogService.getWorkOutByClientId).toHaveBeenCalled();
    });
  });

  it('should filter logs when search input is provided', () => {
    component.allWorkoutLogs = [
      {
        workOutPlanId: 'plan1',
        workOutPlanName: 'Morning Routine',
        userId: 'user1',
        userName: 'John Doe',
        type: 'Cardio',
        date: '2023-01-01',
        duration: '30 min',
      } as any,
    ];
    component.onSearchInput('morning');
    expect(component.filteredWorkoutLogs.length).toBe(1);
  });

  it('should open view log modal', () => {
    const log = { workOutPlanName: 'Test' } as any;
    component.onClickViewLog(log);
    expect(component.selectedLog).toBe(log);
    expect(component.showViewLogModal).toBeTrue();
  });

  it('should add a log if form is valid', () => {
    component.addWorkLog.setValue({
      name: 'user1',
      plan: 'plan1',
      workoutType: 'Cardio',
      date: '2023-01-01',
      duration: '30 min',
      calories: '100',
    });
    component.addNewLog();
    expect(mockWorkOutLogService.addNewWorkOutLog).toHaveBeenCalled();
  });

  it('should update page numbers correctly', () => {
    component.totalLogs = 25;
    const pages = component.updatePageNumbers();
    expect(pages.length).toBeGreaterThan(0);
  });

  it('should change page and load logs', () => {
    component.totalLogs = 25;
    component.changePage(2);
    expect(component.LogFilter.pageNumber).toBe(2);
    expect(mockWorkOutLogService.getAllWorkOutLogsWithPagination).toHaveBeenCalled();
  });

  it('should not change to invalid page', () => {
    component.totalLogs = 5;
    component.changePage(0);
    expect(component.LogFilter.pageNumber).toBe(1);
  });

  it('should load plans options', () => {
    component.getAllPlansOptions();
    expect(mockWorkOutPlanService.getWorkOutPlans).toHaveBeenCalled();
  });

});
