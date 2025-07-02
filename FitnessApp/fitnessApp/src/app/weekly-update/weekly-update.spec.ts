import { ComponentFixture, TestBed } from '@angular/core/testing';
import { WeeklyUpdate } from './weekly-update';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { of, throwError } from 'rxjs';
import { ProgressService } from '../service/progress.service';
import { TokenService } from '../service/token.service';
import { WorkOutPlanService } from '../service/workout-plan.service';
import { Popup } from '../popup/popup';

describe('WeeklyUpdate (Standalone Component)', () => {
  let component: WeeklyUpdate;
  let fixture: ComponentFixture<WeeklyUpdate>;

  let mockProgressService: jasmine.SpyObj<ProgressService>;
  let mockTokenService: jasmine.SpyObj<TokenService>;
  let mockWorkOutPlanService: jasmine.SpyObj<WorkOutPlanService>;

  beforeEach(async () => {
    mockProgressService = jasmine.createSpyObj('ProgressService', [
      'getAllProgressWithPagination',
      'getProgressByCoachId',
      'getProgressByUserId',
      'addNewProgress',
      'addProgressImage',
    ]);
    mockTokenService = jasmine.createSpyObj('TokenService', [
      'getRole',
      'getUserId',
      'getUsername',
    ]);
    mockWorkOutPlanService = jasmine.createSpyObj('WorkOutPlanService', [
      'getWorkOutPlans',
    ]);

    mockProgressService.getAllProgressWithPagination.and.returnValue(
      of({ data: [], totalCount: 0 })
    );
    mockProgressService.getProgressByCoachId.and.returnValue(of([]));
    mockProgressService.getProgressByUserId.and.returnValue(of([]));
    mockProgressService.addNewProgress.and.returnValue(of({ id: 'progress1' }));
    mockProgressService.addProgressImage.and.returnValue(of({}));
    mockWorkOutPlanService.getWorkOutPlans.and.returnValue(of([]));

    mockTokenService.getRole.and.returnValue('Admin');
    mockTokenService.getUserId.and.returnValue('user1');
    mockTokenService.getUsername.and.returnValue('John Doe');

    await TestBed.configureTestingModule({
      imports: [WeeklyUpdate, ReactiveFormsModule, FormsModule, CommonModule],
      providers: [
        { provide: ProgressService, useValue: mockProgressService },
        { provide: TokenService, useValue: mockTokenService },
        { provide: WorkOutPlanService, useValue: mockWorkOutPlanService },
        { provide: Popup, useValue: { display: jasmine.createSpy('display') } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(WeeklyUpdate);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should load progress for Admin', () => {
      component.ngOnInit();
      expect(
        mockProgressService.getAllProgressWithPagination
      ).toHaveBeenCalled();
    });

    it('should load progress for Coach', () => {
      mockTokenService.getRole.and.returnValue('Coach');
      component.ngOnInit();
      expect(mockProgressService.getProgressByCoachId).toHaveBeenCalled();
    });

    it('should load progress for User', () => {
      mockTokenService.getRole.and.returnValue('User');
      component.ngOnInit();
      expect(mockProgressService.getProgressByUserId).toHaveBeenCalled();
    });
  });

  it('should open progress details modal', () => {
    const progress = { id: 'p1' };
    component.viewProgressDetails(progress);
    expect(component.selectedProgress).toBe(progress);
    expect(component.showProgressDetailsModal).toBeTrue();
  });

  it('should set selectedImage when onImageSelected()', () => {
    const file = new File(['dummy'], 'image.png', { type: 'image/png' });
    const event = {
      target: { files: [file] },
    } as unknown as Event;
    component.onImageSelected(event);
    expect(component.selectedImage).toBe(file);
  });

  describe('addNewProgress()', () => {
    it('should add progress successfully and upload image', () => {
      component.selectedImage = new File(['dummy'], 'image.png', {
        type: 'image/png',
      });
      component.addProgress.setValue({
        name: 'user1',
        workOutPlan: 'plan1',
        submissionDate: '2023-01-01',
        bodyWeight: '70',
        fatPercentage: '15',
        muscleMass: '30',
        waterPercentage: '60',
      });
      component.addNewProgress();
      expect(mockProgressService.addProgressImage).toHaveBeenCalled();
    });
  });

  it('should update page numbers correctly', () => {
    component.totalProgress = 25;
    const pages = component.UpdatePageNumbers();
    expect(pages.length).toBeGreaterThan(0);
  });

  it('should change page and reload', () => {
    component.totalProgress = 25;
    component.changePage(2);
    expect(component.progressFilters.pageNumber).toBe(2);
    expect(mockProgressService.getAllProgressWithPagination).toHaveBeenCalled();
  });

  it('should not change to invalid page', () => {
    component.totalProgress = 5;
    component.changePage(0);
    expect(component.progressFilters.pageNumber).toBe(1);
  });
});
