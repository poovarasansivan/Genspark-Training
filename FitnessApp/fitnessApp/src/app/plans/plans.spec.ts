import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Plans } from './plans';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';

import { WorkOutPlanService } from '../service/workout-plan.service';
import { UserService } from '../service/user.service';
import { UserPlanService } from '../service/user-plan.service';
import { TokenService } from '../service/token.service';
import { Popup } from '../popup/popup';

describe('Plans', () => {
  let component: Plans;
  let fixture: ComponentFixture<Plans>;

  let mockWorkOutPlanService: jasmine.SpyObj<WorkOutPlanService>;
  let mockUserService: jasmine.SpyObj<UserService>;
  let mockUserPlanService: jasmine.SpyObj<UserPlanService>;
  let mockTokenService: jasmine.SpyObj<TokenService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    mockWorkOutPlanService = jasmine.createSpyObj('WorkOutPlanService', [
      'getWorkOutPlansByPagination',
      'getWorkOutPlanByCoachId',
      'getWorkOutPlanByUserId',
      'getWorkOutPlans',
      'AddNewWorkOutPlan',
    ]);
    mockUserService = jasmine.createSpyObj('UserService', ['getUserOptions']);
    mockUserPlanService = jasmine.createSpyObj('UserPlanService', ['addUserPlan']);
    mockTokenService = jasmine.createSpyObj('TokenService', ['getRole', 'getUserId']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    mockWorkOutPlanService.getWorkOutPlansByPagination.and.returnValue(of({ data: [], totalCount: 0 }));
    mockWorkOutPlanService.getWorkOutPlanByCoachId.and.returnValue(of([]));
    mockWorkOutPlanService.getWorkOutPlanByUserId.and.returnValue(of([]));
    mockWorkOutPlanService.getWorkOutPlans.and.returnValue(of([]));
    mockWorkOutPlanService.AddNewWorkOutPlan.and.returnValue(of({}));
    mockUserService.getUserOptions.and.returnValue(of({ data: { $values: [] } }));
    mockUserPlanService.addUserPlan.and.returnValue(of({}));
    mockTokenService.getRole.and.returnValue('Admin');
    mockTokenService.getUserId.and.returnValue('user-id');

    await TestBed.configureTestingModule({
      imports: [
        Plans,
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
      ],
      providers: [
        { provide: WorkOutPlanService, useValue: mockWorkOutPlanService },
        { provide: UserService, useValue: mockUserService },
        { provide: UserPlanService, useValue: mockUserPlanService },
        { provide: TokenService, useValue: mockTokenService },
        { provide: Router, useValue: mockRouter },
      ],
    }).overrideComponent(Plans, {
      set: {
        providers: [
          {
            provide: Popup,
            useValue: {
              display: jasmine.createSpy('display'),
            },
          },
        ],
      },
    }).compileComponents();

    fixture = TestBed.createComponent(Plans);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should load plans for Admin role', () => {
      component.ngOnInit();
      expect(mockWorkOutPlanService.getWorkOutPlansByPagination).toHaveBeenCalled();
      expect(mockWorkOutPlanService.getWorkOutPlans).toHaveBeenCalled();
      expect(mockUserService.getUserOptions).toHaveBeenCalled();
    });

    it('should load plans for Coach role', () => {
      mockTokenService.getRole.and.returnValue('Coach');
      component.ngOnInit();
      expect(mockWorkOutPlanService.getWorkOutPlanByCoachId).toHaveBeenCalled();
    });

    it('should load plans for User role', () => {
      mockTokenService.getRole.and.returnValue('User');
      component.ngOnInit();
      expect(mockWorkOutPlanService.getWorkOutPlanByUserId).toHaveBeenCalled();
    });
  });

  describe('getAllPlans', () => {
    it('should fetch plans with pagination', () => {
      component.getAllPlans();
      expect(mockWorkOutPlanService.getWorkOutPlansByPagination).toHaveBeenCalled();
    });
  });

  describe('addNewPlan', () => {
    it('should not add plan if form invalid', () => {
      component.newPlanForm.patchValue({
        planName: '',
        planDescription: '',
        startDate: '',
        endDate: '',
      });
      component.addNewPlan();
      expect(mockWorkOutPlanService.AddNewWorkOutPlan).not.toHaveBeenCalled();
    });

    it('should add plan if form valid', () => {
      component.newPlanForm.patchValue({
        planName: 'Test Plan',
        planDescription: 'Description',
        startDate: '2024-01-01',
        endDate: '2024-01-10',
      });
      component.addNewPlan();
      expect(mockWorkOutPlanService.AddNewWorkOutPlan).toHaveBeenCalled();
    });

    it('should handle error when adding plan', () => {
      mockWorkOutPlanService.AddNewWorkOutPlan.and.returnValue(throwError(() => new Error('Add error')));
      component.newPlanForm.patchValue({
        planName: 'Test Plan',
        planDescription: 'Description',
        startDate: '2024-01-01',
        endDate: '2024-01-10',
      });
      component.addNewPlan();
      expect(mockWorkOutPlanService.AddNewWorkOutPlan).toHaveBeenCalled();
    });
  });

  describe('addNewPlanEnroll', () => {
    it('should not enroll if form invalid', () => {
      component.newPlanEnrollForm.patchValue({
        clientId: '',
        planId: '',
      });
      component.addNewPlanEnroll();
      expect(mockUserPlanService.addUserPlan).not.toHaveBeenCalled();
    });

    it('should enroll if form valid', () => {
      component.newPlanEnrollForm.patchValue({
        clientId: 'client1',
        planId: 'plan1',
      });
      component.addNewPlanEnroll();
      expect(mockUserPlanService.addUserPlan).toHaveBeenCalledWith('client1', 'plan1');
    });

    it('should handle error on enrollment', () => {
      mockUserPlanService.addUserPlan.and.returnValue(throwError(() => new Error('Enroll error')));
      component.newPlanEnrollForm.patchValue({
        clientId: 'client1',
        planId: 'plan1',
      });
      component.addNewPlanEnroll();
      expect(mockUserPlanService.addUserPlan).toHaveBeenCalled();
    });
  });

  it('should navigate to plan details', () => {
    component.viewPlan('plan-123');
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/manage-plan/plan-details', 'plan-123']);
  });

  it('should change sort order and call getAllPlans', () => {
    component.onSortOrderChange('desc');
    expect(component.sortOrder).toBe('desc');
    expect(mockWorkOutPlanService.getWorkOutPlansByPagination).toHaveBeenCalled();
  });

  it('should change page and call getAllPlans', () => {
    component.onPageChange(3);
    expect(component.currentPage).toBe(3);
    expect(mockWorkOutPlanService.getWorkOutPlansByPagination).toHaveBeenCalled();
  });
});
