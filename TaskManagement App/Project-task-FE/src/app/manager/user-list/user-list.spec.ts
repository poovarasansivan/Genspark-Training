import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UserListComponent } from './user-list';
import { UserService } from '../Create-User/user.service';
import { ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';
import { UserResponseDto } from '../Create-User/user.types';

describe('UserListComponent', () => {
  let component: UserListComponent;
  let fixture: ComponentFixture<UserListComponent>;
  let userServiceMock: jasmine.SpyObj<UserService>;
  let toastrMock: jasmine.SpyObj<ToastrService>;

  const mockUsers: UserResponseDto[] = [
  {
    id: '1',
    fullName: 'Alice Smith',
    email: 'alice@example.com',
    role: 'Manager',
    createdAt: '2024-07-01T12:00:00Z'
  },
  {
    id: '2',
    fullName: 'Bob Johnson',
    email: 'bob@example.com',
    role: 'TeamMember',
    createdAt: '2024-07-02T14:30:00Z'
  }
];

  beforeEach(async () => {
    userServiceMock = jasmine.createSpyObj('UserService', ['getAllUsers', 'deleteUser']);
    toastrMock = jasmine.createSpyObj('ToastrService', ['success', 'error']);

    userServiceMock.getAllUsers.and.returnValue(of({
    data: mockUsers,
    message: 'Success',
    pagination: {
      page: 1,
      pageSize: 5,
      totalPages: 1,
      totalRecords: 2
    }
  }));

    await TestBed.configureTestingModule({
      imports: [UserListComponent],
      providers: [
        { provide: UserService, useValue: userServiceMock },
        { provide: ToastrService, useValue: toastrMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges(); // triggers ngOnInit
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch users on init', () => {
    expect(userServiceMock.getAllUsers).toHaveBeenCalled();
    expect(component.users.length).toBe(2);
    expect(component.totalPages).toBe(1);
  });

  it('should delete user and refresh list', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    userServiceMock.deleteUser.and.returnValue(of({}));

    component.deleteUser('1');

    expect(userServiceMock.deleteUser).toHaveBeenCalledWith('1');
    expect(toastrMock.success).toHaveBeenCalledWith('User deleted');
  });

  it('should show error if user delete fails', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    userServiceMock.deleteUser.and.returnValue(throwError(() => ({
      error: { message: 'Delete failed' }
    })));

    component.deleteUser('1');

    expect(toastrMock.error).toHaveBeenCalledWith('Delete failed');
  });


  it('should return initials from full name', () => {
    const initials = component.getInitials('John Doe');
    expect(initials).toBe('JD');
  });
});
