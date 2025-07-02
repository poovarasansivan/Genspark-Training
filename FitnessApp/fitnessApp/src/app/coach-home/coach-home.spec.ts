import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CoachHome } from './coach-home';
import { UserService } from '../service/user.service';
import { TokenService } from '../service/token.service';
import { UserPlanService } from '../service/user-plan.service';
import { of } from 'rxjs';

describe('CoachHome', () => {
  let component: CoachHome;
  let fixture: ComponentFixture<CoachHome>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let tokenServiceSpy: jasmine.SpyObj<TokenService>;
  let userPlanServiceSpy: jasmine.SpyObj<UserPlanService>;

  const mockWorkoutPlansResponse = [
    {
      planId: 'plan1',
      planName: 'Plan 1',
      usersCount: 1,
      completedUsers: 2,
      startDate: '2025-01-01',
      endDate: '2025-02-01',
    },
    {
      planId: 'plan2',
      planName: 'Plan 2',
      usersCount: 1,
      completedUsers: 0,
      startDate: '2025-03-01',
      endDate: '2025-04-01',
    },
    {
      planId: 'plan3',
      planName: 'Plan 3',
      usersCount: 0,
      completedUsers: 0,
      startDate: '2025-05-01',
      endDate: '2025-06-01',
    },
  ];

  const mockUserPlansResponse = {
    data: {
      $values: [
        {
          coachId: 'coach123',
          workOutPlanId: 'plan1',
          workOutPlanName: 'Fat Loss Plan',
          isCompleted: 'Completed',
          startDate: '2025-01-01',
          endDate: '2025-02-01',
        },
        {
          coachId: 'coach123',
          workOutPlanId: 'plan1',
          workOutPlanName: 'Fat Loss Plan',
          isCompleted: 'OnGoing',
          startDate: '2025-01-01',
          endDate: '2025-02-01',
        },
        {
          coachId: 'coach123',
          workOutPlanId: 'plan2',
          workOutPlanName: 'Muscle Gain Plan',
          isCompleted: 'OnGoing',
          startDate: '2025-03-01',
          endDate: '2025-04-01',
        },
        {
          coachId: 'otherCoach',
          workOutPlanId: 'plan2',
          workOutPlanName: 'Muscle Gain Plan',
          isCompleted: 'Completed',
          startDate: '2025-03-01',
          endDate: '2025-04-01',
        },
      ],
    },
  };

  beforeEach(waitForAsync(() => {
    tokenServiceSpy = jasmine.createSpyObj('TokenService', [
      'getUsername',
      'getUserId',
    ]);
    userServiceSpy = jasmine.createSpyObj('UserService', ['getPlanAnalysis']);
    userPlanServiceSpy = jasmine.createSpyObj('UserPlanService', [
      'getWorkOutPlans',
    ]);

    tokenServiceSpy.getUsername.and.returnValue('Coach John');
    tokenServiceSpy.getUserId.and.returnValue('coach123');

    userPlanServiceSpy.getWorkOutPlans.and.returnValue(
      of(mockWorkoutPlansResponse)
    );

    userServiceSpy.getPlanAnalysis.and.returnValue(of(mockUserPlansResponse));

    TestBed.configureTestingModule({
      imports: [CoachHome],
      providers: [
        { provide: TokenService, useValue: tokenServiceSpy },
        { provide: UserService, useValue: userServiceSpy },
        { provide: UserPlanService, useValue: userPlanServiceSpy },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CoachHome);
    component = fixture.componentInstance;
  });
  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  it('should get username and set coachName', () => {
    const username = component.getUsername();
    expect(username).toBe('Coach John');
    expect(component.coachName).toBe('Coach John');
  });

  it('should handle null username gracefully', () => {
    tokenServiceSpy.getUsername.and.returnValue(null);
    const username = component.getUsername();
    expect(username).toBeNull();
    expect(component.coachName).toBe('');
  });

  it('should process plans and set counts correctly on ngOnInit', async () => {
    component.ngOnInit();

    await fixture.whenStable();

    expect(component.uniquePlansCount).toBe(2);
    expect(component.activeUsersCount).toBe(2);
    expect(component.completedUsersCount).toBe(1);

    expect(component.plans.length).toBe(2);

    const plan1 = component.plans.find((p) => p.planId === 'plan1');
    expect(plan1?.usersCount).toBe(2);
    expect(plan1?.completedUsers).toBe(1);
    expect(plan1?.planName).toBe('Fat Loss Plan');

    const plan2 = component.plans.find((p) => p.planId === 'plan2');
    expect(plan2?.usersCount).toBe(1);
    expect(plan2?.completedUsers).toBe(0);
    expect(plan2?.planName).toBe('Muscle Gain Plan');
  });

  it('should handle empty responses gracefully', async () => {
    userPlanServiceSpy.getWorkOutPlans.and.returnValue(of([]));
    userServiceSpy.getPlanAnalysis.and.returnValue(
      of({ data: { $values: [] } })
    );

    component.ngOnInit();

    await fixture.whenStable();

    expect(component.plans.length).toBe(0);
    expect(component.activeUsersCount).toBe(0);
    expect(component.completedUsersCount).toBe(0);
    expect(component.uniquePlansCount).toBe(0);
  });
});
