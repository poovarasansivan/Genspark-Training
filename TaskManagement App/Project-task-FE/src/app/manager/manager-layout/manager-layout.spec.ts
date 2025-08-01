import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ManagerLayoutComponent } from './manager-layout';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../auth/auth.service';
import { of, BehaviorSubject } from 'rxjs';
import { NotificationService } from '../../Notifications/notification.service';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../Create-User/user.service';
import { TaskService } from '../create-task/task.service';
import { RouterTestingModule } from '@angular/router/testing';

@Component({
  standalone: true,
  template: ''
})
class DummyComponent {}

describe('ManagerLayoutComponent', () => {
  let component: ManagerLayoutComponent;
  let fixture: ComponentFixture<ManagerLayoutComponent>;
  let router: Router;

  beforeEach(async () => {
    const notificationsSubject = new BehaviorSubject<any[]>([]);
    const notificationServiceMock = {
      notifications$: notificationsSubject.asObservable()
    };

    const toastrServiceMock = jasmine.createSpyObj('ToastrService', ['success', 'error']);

    const userServiceMock = jasmine.createSpyObj('UserService', ['getAllUsers', 'createUser', 'deleteUser']);
    userServiceMock.getAllUsers.and.returnValue(of({ data: [], pagination: { totalPages: 1 } }));
    userServiceMock.createUser.and.returnValue(of({}));
    userServiceMock.deleteUser.and.returnValue(of({}));

    const taskServiceMock = jasmine.createSpyObj('TaskService', ['getAllTasks', 'createTask', 'deleteTask', 'searchTasks']);
    taskServiceMock.getAllTasks.and.returnValue(of({ data: [], pagination: { totalPages: 1 } }));
    taskServiceMock.createTask.and.returnValue(of({}));
    taskServiceMock.deleteTask.and.returnValue(of({}));
    taskServiceMock.searchTasks.and.returnValue(of({ data: [] }));

    const authServiceSpy = jasmine.createSpyObj('AuthService', ['getUser', 'getUserId']);
    authServiceSpy.getUser.and.returnValue(of({ id: '1', role: 'Manager' }));
    authServiceSpy.getUserId.and.returnValue('1');

    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        RouterTestingModule.withRoutes([
          { path: 'dashboard', component: DummyComponent },
          { path: 'access-denied', component: DummyComponent },
          { path: 'not-found', component: DummyComponent },
          { path: '404', component: DummyComponent }
        ]),
        DummyComponent, // <-- required for standalone route target
        ManagerLayoutComponent
      ],
      providers: [
        { provide: ActivatedRoute, useValue: {} },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: NotificationService, useValue: notificationServiceMock },
        { provide: ToastrService, useValue: toastrServiceMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: TaskService, useValue: taskServiceMock }
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    fixture = TestBed.createComponent(ManagerLayoutComponent);
    component = fixture.componentInstance;
    await router.navigateByUrl('/dashboard');
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show sidebar for /dashboard', () => {
    expect(component.shouldShowSidebar).toBeTrue();
  });

  it('should not show sidebar for /access-denied', async () => {
    await router.navigateByUrl('/access-denied');
    fixture.detectChanges();
    expect(component.shouldShowSidebar).toBeFalse();
  });

  it('should not show sidebar for /not-found', async () => {
    await router.navigateByUrl('/not-found');
    fixture.detectChanges();
    expect(component.shouldShowSidebar).toBeFalse();
  });

  it('should not show sidebar for /404', async () => {
    await router.navigateByUrl('/404');
    fixture.detectChanges();
    expect(component.shouldShowSidebar).toBeFalse();
  });
});
