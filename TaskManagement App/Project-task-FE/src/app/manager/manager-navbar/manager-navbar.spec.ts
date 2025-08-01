// import { ComponentFixture, TestBed } from '@angular/core/testing';
// import { ManagerNavbarComponent } from './manager-navbar';
// import { of } from 'rxjs';
// import { AuthService } from '../../auth/auth.service';
// import { NotificationService } from '../../Notifications/notification.service';
// import { Router } from '@angular/router';
// import { ActivatedRoute } from '@angular/router';

// describe('ManagerNavbarComponent', () => {
//   let component: ManagerNavbarComponent;
//   let fixture: ComponentFixture<ManagerNavbarComponent>;

//   const mockUser = {
//     id: '1',
//     role: 'Manager',
//     // fullName: 'Test Manager'
//   };

//   const authServiceMock = jasmine.createSpyObj('AuthService', ['logout']);
//   const routerMock = jasmine.createSpyObj('Router', ['navigate']);

//   // ✅ Ensure this is an observable using `of()`
//   const notificationServiceMock = {
//     notifications$: of([
//       { id: 1, isRead: false },
//       { id: 2, isRead: true },
//       { id: 3, isRead: false }
//     ])
//   };

//   beforeEach(async () => {
//     // ✅ Mock sessionStorage
//     spyOn(sessionStorage, 'getItem').and.callFake((key: string) => {
//       if (key === 'user') return JSON.stringify(mockUser);
//       return null;
//     });

//     await TestBed.configureTestingModule({
//       imports: [ManagerNavbarComponent],
//       providers: [
//         { provide: AuthService, useValue: authServiceMock },
//         { provide: Router, useValue: routerMock },
//         { provide: NotificationService, useValue: notificationServiceMock },
//         { provide: ActivatedRoute, useValue: {} }
//       ]
//     }).compileComponents();

//     fixture = TestBed.createComponent(ManagerNavbarComponent);
//     component = fixture.componentInstance;
//     fixture.detectChanges(); // ✅ This will run ngOnInit
//   });

//   it('should create', () => {
//     expect(component).toBeTruthy();
//   });

//   it('should count unread notifications correctly', () => {
//     expect(component.unreadCount).toBe(2); // 2 unread
//   });

//   it('should toggle sidebar state', () => {
//     expect(component.isSidebarOpen).toBeFalse();
//     component.toggleSidebar();
//     expect(component.isSidebarOpen).toBeTrue();
//     component.toggleSidebar();
//     expect(component.isSidebarOpen).toBeFalse();
//   });

//   it('should call AuthService.logout() on logout()', () => {
//     component.logout();
//     expect(authServiceMock.logout).toHaveBeenCalled();
//   });

//   it('should identify manager role', () => {
//     expect(component.isManager()).toBeTrue();
//     expect(component.isTeamMember()).toBeFalse();
//   });
// });
