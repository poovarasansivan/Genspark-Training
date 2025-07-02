import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClientHome } from './client-home';
import { UserPlanService } from '../service/user-plan.service';
import { TokenService } from '../service/token.service';
import { of } from 'rxjs';

describe('ClientHome', () => {
  let component: ClientHome;
  let fixture: ComponentFixture<ClientHome>;
  let userPlanServiceSpy: jasmine.SpyObj<UserPlanService>;
  let tokenServiceSpy: jasmine.SpyObj<TokenService>;

  const mockUserPlansResponse = [
    {
      id: '123',
      userId: 'user123',
      userName: 'Client A',
      workOutPlanId: 'plan1',
      workOutPlanName: 'Fat Loss Plan',
      coachId: 'coach123',
      coachName: 'Coach John',
      isCompleted: 'Completed',
      startDate: '2025-01-01',
      endDate: '2025-02-01',
    },
    {
      id: '124',
      userId: 'user123',
      userName: 'Client A',
      workOutPlanId: 'plan1',
      workOutPlanName: 'Fat Loss Plan',
      coachId: 'coach123',
      coachName: 'Coach John',
      isCompleted: 'OnGoing',
      startDate: '2025-01-01',
      endDate: '2025-02-01',
    },
  ];

  beforeEach(async () => {
    tokenServiceSpy = jasmine.createSpyObj('TokenService', ['getUsername','getUserId']);
    userPlanServiceSpy = jasmine.createSpyObj('UserPlanService', [
      'getAllWorkOutPlans',
    ]);
    await TestBed.configureTestingModule({
      imports: [ClientHome],
      providers: [
        {
          provide: UserPlanService,
          useValue: userPlanServiceSpy,
        },
        {
          provide: TokenService,
          useValue: tokenServiceSpy,
        },
      ],
    }).compileComponents();

    tokenServiceSpy.getUserId.and.returnValue('user123');
    tokenServiceSpy.getUsername.and.returnValue('Coach John');
    userPlanServiceSpy.getAllWorkOutPlans.and.returnValue(
      of(mockUserPlansResponse)
    );

    fixture = TestBed.createComponent(ClientHome);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create client home component', () => {
    expect(component).toBeTruthy();
  });

  it('should set client name from token service', () => {
    expect(component.clientName).toBe('Coach John');
  });

  it('should filter plans by userId', () => {
    const userId = 'user123';
    const allPlans = mockUserPlansResponse;
    const userPlans = allPlans.filter((plan) => plan.userId === userId);
    expect(userPlans.length).toBe(2);
    expect(userPlans[0].userName).toBe('Client A');
  });

  it('should separate completed and active plans', () => {
    const userId = 'user123';
    const allPlans = mockUserPlansResponse;
    const userPlans = allPlans.filter((plan) => plan.userId === userId);
    
    const completedPlans = userPlans.filter((plan) => plan.isCompleted === 'Completed');
    const activePlans = userPlans.filter((plan) => plan.isCompleted !== 'Completed');

    expect(completedPlans.length).toBe(1);
    expect(activePlans.length).toBe(1);
  });
});
