import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ManageUser, passwordMatchValidator } from './manage-user';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { of, throwError } from 'rxjs';
import { Popup } from '../popup/popup';
import { UserService } from '../service/user.service';

describe('ManageUser', () => {
  let component: ManageUser;
  let fixture: ComponentFixture<ManageUser>;
  let userServiceSpy: jasmine.SpyObj<any>;

  beforeEach(async () => {
    userServiceSpy = jasmine.createSpyObj('UserService', [
      'getUsersPagination',
      'addNewUser',
      'deleteUser',
      'updateUser',
    ]);

    await TestBed.configureTestingModule({
      imports: [ManageUser, ReactiveFormsModule, FormsModule, CommonModule],
      providers: [
        { provide: UserService, useValue: userServiceSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ManageUser);
    component = fixture.componentInstance;
    // Mock the popup
    component.toast = jasmine.createSpyObj('Popup', ['display']);
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should call loadAllUsers', () => {
      spyOn(component, 'loadAllUsers');
      component.ngOnInit();
      expect(component.loadAllUsers).toHaveBeenCalled();
    });
  });

  describe('loadAllUsers', () => {
    it('should load users and set pagination', () => {
      const mockUsers = Array.from({ length: 12 }, (_, i) => ({
        id: `${i}`,
        name: `User${i}`,
        email: `user${i}@example.com`,
        role: 'User',
        isActive: true,
      }));

      userServiceSpy.getUsersPagination.and.returnValue(
        of({ data: mockUsers, totalCount: 12 })
      );

      component.loadAllUsers();

      expect(component.allUsers.length).toBe(12);
      expect(component.pageNumbers.length).toBeGreaterThan(0);
    });
  });

  describe('filterUsersClientSide', () => {
    beforeEach(() => {
      component.allUsers = [
        { id: '1', name: 'Alice', email: 'alice@example.com', role: 'Admin', isActive: true },
        { id: '2', name: 'Bob', email: 'bob@example.com', role: 'User', isActive: false },
      ];
    });

    it('should filter by search term', () => {
      component.searchTerm = 'alice';
      component.filterUsersClientSide();
      expect(component.users.length).toBe(1);
    });

    it('should filter by role', () => {
      component.searchTerm = '';
      component.roleFilter = 'Admin';
      component.filterUsersClientSide();
      expect(component.users.length).toBe(1);
    });

    it('should filter by status', () => {
      component.statusFilter = 'true';
      component.filterUsersClientSide();
      expect(component.users.length).toBe(1);
    });
  });

  describe('addUser', () => {
    it('should show error if form invalid', () => {
      component.addUserForm.setValue({
        name: '',
        email: '',
        password: '',
        confirmPassword: '',
        role: '',
        status: '',
      });
      component.addUser();
      expect(component.toast.display).toHaveBeenCalledWith(
        'Please fill in all required fields correctly.',
        'error'
      );
    });

    it('should call addNewUser and reset form', () => {
      component.addUserForm.setValue({
        name: 'Test User',
        email: 'test@example.com',
        password: '123456',
        confirmPassword: '123456',
        role: 'User',
        status: 'true',
      });

      userServiceSpy.addNewUser.and.returnValue(of({}));

      spyOn(component, 'loadAllUsers');

      component.addUser();

      expect(userServiceSpy.addNewUser).toHaveBeenCalled();
      expect(component.loadAllUsers).toHaveBeenCalled();
      expect(component.toast.display).toHaveBeenCalledWith(
        'User added successfully',
        'success'
      );
      expect(component.showAddUserModal).toBeFalse();
    });

    it('should handle error when adding user', () => {
      component.addUserForm.setValue({
        name: 'Test User',
        email: 'test@example.com',
        password: '123456',
        confirmPassword: '123456',
        role: 'User',
        status: 'true',
      });
      userServiceSpy.addNewUser.and.returnValue(
        throwError(() => new Error('Error'))
      );

      component.addUser();

      expect(component.toast.display).toHaveBeenCalledWith(
        'Failed to add user',
        'error'
      );
    });
  });

  describe('deleteUser', () => {
    it('should delete user and reload', () => {
      component.selectedUser = { id: '1' } as any;
      userServiceSpy.deleteUser.and.returnValue(of({}));

      spyOn(component, 'loadAllUsers');

      component.deleteUser();

      expect(userServiceSpy.deleteUser).toHaveBeenCalledWith('1');
      expect(component.loadAllUsers).toHaveBeenCalled();
      expect(component.toast.display).toHaveBeenCalledWith(
        'User deleted successfully.',
        'success'
      );
      expect(component.showDeleteUserModal).toBeFalse();
    });

    it('should handle error when deleting user', () => {
      component.selectedUser = { id: '1' } as any;
      userServiceSpy.deleteUser.and.returnValue(
        throwError(() => new Error('Error'))
      );

      component.deleteUser();

      expect(component.toast.display).toHaveBeenCalledWith(
        'Failed to delete user.',
        'error'
      );
    });
  });

  describe('editUser', () => {
    beforeEach(() => {
      component.selectedUser = {
        id: '1',
        name: 'Old Name',
        email: 'old@example.com',
        role: 'User',
        isActive: true,
      } as any;

      component.editUserForm.setValue({
        ename: 'New Name',
        eemail: 'new@example.com',
        erole: 'Admin',
        estatus: 'false',
      });
    });

    it('should update user and reload', () => {
      userServiceSpy.updateUser.and.returnValue(of({}));
      spyOn(component, 'loadAllUsers');
      spyOn(component, 'closeModals');

      component.editUser();

      expect(userServiceSpy.updateUser).toHaveBeenCalledWith('1', jasmine.objectContaining({
        name: 'New Name',
        email: 'new@example.com',
        role: 'Admin',
        isActive: false,
      }));
      expect(component.loadAllUsers).toHaveBeenCalled();
      expect(component.closeModals).toHaveBeenCalled();
      expect(component.toast.display).toHaveBeenCalledWith(
        'User updated successfully',
        'success'
      );
    });

    it('should handle error when updating user', () => {
      userServiceSpy.updateUser.and.returnValue(
        throwError(() => new Error('Error'))
      );

      component.editUser();

      expect(component.toast.display).toHaveBeenCalledWith(
        'Failed to update user',
        'error'
      );
    });
  });
});
