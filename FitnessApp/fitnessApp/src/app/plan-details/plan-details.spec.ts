import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PlanDetails } from './plan-details';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { of, throwError } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { WorkOutPlanService } from '../service/workout-plan.service';
import { DataService } from '../service/data.service';
import { TokenService } from '../service/token.service';
import { Popup } from '../popup/popup';

describe('PlanDetails', () => {
  let component: PlanDetails;
  let fixture: ComponentFixture<PlanDetails>;

  let mockWorkOutPlanService: jasmine.SpyObj<WorkOutPlanService>;
  let mockTokenService: jasmine.SpyObj<TokenService>;
  let mockRouter: jasmine.SpyObj<Router>;
  let mockDataService: jasmine.SpyObj<DataService>;
  let mockActivatedRoute: any;

  beforeEach(async () => {
    mockWorkOutPlanService = jasmine.createSpyObj('WorkOutPlanService', [
      'getGroupedWorkOutPlans',
      'getWorkOutPlanByCoachId',
      'getWorkOutPlanByUserId',
      'updateWorkOutPlan',
    ]);
    mockTokenService = jasmine.createSpyObj('TokenService', ['getRole', 'getUserId']);
    mockRouter = jasmine.createSpyObj('Router', ['navigateByUrl']);
    mockDataService = jasmine.createSpyObj('DataService', ['setUser']);

    // Route params
    mockActivatedRoute = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue('plan123'),
        },
      },
    };

    // Default responses
    mockWorkOutPlanService.getGroupedWorkOutPlans.and.returnValue(of([{ id: 'plan123' }]));
    mockWorkOutPlanService.getWorkOutPlanByCoachId.and.returnValue(of([{ id: 'plan123' }]));
    mockWorkOutPlanService.getWorkOutPlanByUserId.and.returnValue(of([{ id: 'plan123' }]));
    mockWorkOutPlanService.updateWorkOutPlan.and.returnValue(of({}));

    mockTokenService.getRole.and.returnValue('Admin');
    mockTokenService.getUserId.and.returnValue('user1');

    await TestBed.configureTestingModule({
      imports: [
        PlanDetails,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
      ],
      providers: [
        { provide: WorkOutPlanService, useValue: mockWorkOutPlanService },
        { provide: TokenService, useValue: mockTokenService },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
        { provide: DataService, useValue: mockDataService },
      ],
    })
      .overrideComponent(PlanDetails, {
        set: {
          providers: [
            { provide: Popup, useValue: { display: jasmine.createSpy('display') } },
          ],
        },
      })
      .compileComponents();

    fixture = TestBed.createComponent(PlanDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should load plans for Admin', () => {
      component.ngOnInit();
      expect(mockWorkOutPlanService.getGroupedWorkOutPlans).toHaveBeenCalledWith('plan123');
    });

    it('should load plans for Coach', () => {
      mockTokenService.getRole.and.returnValue('Coach');
      component.ngOnInit();
      expect(mockWorkOutPlanService.getWorkOutPlanByCoachId).toHaveBeenCalledWith('user1');
    });

    it('should load plans for User', () => {
      mockTokenService.getRole.and.returnValue('User');
      component.ngOnInit();
      expect(mockWorkOutPlanService.getWorkOutPlanByUserId).toHaveBeenCalledWith('user1');
    });
  });

  it('should call getAllPlanDetails()', () => {
    component.getAllPlanDetails();
    expect(mockWorkOutPlanService.getGroupedWorkOutPlans).toHaveBeenCalledWith('plan123');
  });

  it('should filter plans on search input', () => {
    component.AllPlanDetails = [
      { planName: 'Plan A', planDescription: 'Desc', startDate: '2023-01-01', endDate: '2023-01-31', coachName: 'Coach', clientName: 'Client', isCompleted: 'Completed' },
      { planName: 'Other Plan', planDescription: 'Other Desc', startDate: '2023-02-01', endDate: '2023-02-28', coachName: 'Coach2', clientName: 'Client2', isCompleted: 'InProgress' },
    ];
    component.onSearchInput('plan a');
    expect(component.filteredPlanDetails.length).toBe(1);
  });

  it('should reset filter when search input is empty', () => {
    component.AllPlanDetails = [{ planName: 'A' }];
    component.onSearchInput('');
    expect(component.filteredPlanDetails).toEqual(component.AllPlanDetails);
  });

  it('should update pagination page numbers correctly', () => {
    component.filteredPlanDetails = Array(25).fill({}); // 25 items
    component.updatePageNumbers();
    expect(component.pageNumbers.length).toBeGreaterThan(0);
  });

  it('should change page and update page numbers', () => {
    component.changePage(2);
    expect(component.currentPage).toBe(2);
  });

  it('should open edit modal', () => {
    const plan = { userWorkOutPlanId: 'id1' };
    component.openEditModal(plan);
    expect(component.selectedUserAndPlan).toBe(plan);
    expect(component.showEditPlanModal).toBeTrue();
  });

  it('should submit edit plan', () => {
    component.selectedUserAndPlan = { userWorkOutPlanId: 'id1' };
    component.editPlanForm.patchValue({ isCompleted: 'Completed' });
    component.onEditPlanSubmit();
    expect(mockWorkOutPlanService.updateWorkOutPlan).toHaveBeenCalledWith('id1', jasmine.any(Object));
  });

  it('should open view modal and navigate', () => {
    const plan = { id: 'p1' };
    component.openViewModal(plan);
    expect(mockDataService.setUser).toHaveBeenCalledWith(plan);
    expect(mockRouter.navigateByUrl).toHaveBeenCalledWith('plan-details/user-stats');
  });
});
