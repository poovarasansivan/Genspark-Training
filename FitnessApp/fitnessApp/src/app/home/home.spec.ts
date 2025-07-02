import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Home } from './home';
import { UserService } from '../service/user.service';
import { TokenService } from '../service/token.service';
import { LucideAngularModule } from 'lucide-angular';
import { BarChart } from '../charts/bar-chart/bar-chart';
import { BarChart2 } from '../charts/bar-chart2/bar-chart2';
import { of, throwError } from 'rxjs';

describe('Home', () => {
  let component: Home;
  let fixture: ComponentFixture<Home>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let tokenServiceSpy: jasmine.SpyObj<TokenService>;

  const mockUsersResponse: any[] = [
    { id: '1', isActive: true, role: 'User' },
    { id: '2', isActive: false, role: 'User' },
    { id: '3', isActive: true, role: 'Coach' },
  ];

  const mockPlansResponse = {
    data: {
      $values: [{ id: '1' }, { id: '2' }],
    },
  };

  beforeEach(async () => {
    userServiceSpy = jasmine.createSpyObj('UserService', [
      'getAllUsers',
      'getPlanCount',
      'getPlanAnalysis',
      'getLogsAnalysis',
    ]);

    tokenServiceSpy = jasmine.createSpyObj('TokenService', ['getUsername']);

    await TestBed.configureTestingModule({
      imports: [Home, LucideAngularModule, BarChart, BarChart2],
      providers: [
        { provide: UserService, useValue: userServiceSpy },
        { provide: TokenService, useValue: tokenServiceSpy },
      ],
    }).compileComponents();

    userServiceSpy.getAllUsers.and.returnValue(of(mockUsersResponse));
    userServiceSpy.getPlanCount.and.returnValue(of(mockPlansResponse));
    userServiceSpy.getPlanAnalysis.and.returnValue(of({}));
    userServiceSpy.getLogsAnalysis.and.returnValue(of({}));

    tokenServiceSpy.getUsername.and.returnValue('Admin User');

    fixture = TestBed.createComponent(Home);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  it('should get username and set AdminName', () => {
    tokenServiceSpy.getUsername.and.returnValue('Admin User');

    const username = component.getUsername();

    expect(username).toBe('Admin User');
    expect(component.AdminName).toBe('Admin User');
  });

  it('should handle null username', () => {
    tokenServiceSpy.getUsername.and.returnValue(null);

    const username = component.getUsername();

    expect(username).toBeNull();
    expect(component.AdminName).toBe('Admin User');
  });

  it('should handle error when fetching plans', () => {
    spyOn(console, 'error');
    tokenServiceSpy.getUsername.and.returnValue('Admin User');
    userServiceSpy.getPlanCount.and.returnValue(
      throwError(() => new Error('Fetch error'))
    );

    component.ngOnInit();

    expect(console.error).toHaveBeenCalledWith(
      'Error fetching plan count:',
      jasmine.any(Error)
    );
  });
});
