import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateTaskComponent } from './create-task';
import { TaskService } from './task.service';
import { UserService } from '../Create-User/user.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';

describe('CreateTaskComponent', () => {
  let component: CreateTaskComponent;
  let fixture: ComponentFixture<CreateTaskComponent>;

  const mockUsers = [
    {
      id: '1',
      fullName: 'Alice Doe',
      email: 'alice@example.com',
      role: 'TeamMember',
      createdAt: '2023-01-01T00:00:00Z'
    }
  ];

  const taskServiceMock = {
    createTask: jasmine.createSpy('createTask')
  };

  const userServiceMock = {
  getAllTeamMembers: jasmine.createSpy('getAllTeamMembers').and.returnValue(of({ data: mockUsers }))
};


  const toastrMock = {
    success: jasmine.createSpy('success'),
    error: jasmine.createSpy('error')
  };

  const routerMock = {
    navigate: jasmine.createSpy('navigate')
  };

  beforeEach(async () => {
    // Reset spy to success behavior before each test
    taskServiceMock.createTask = jasmine.createSpy('createTask').and.returnValue(of({}));

    await TestBed.configureTestingModule({
      imports: [CreateTaskComponent],
      providers: [
        { provide: TaskService, useValue: taskServiceMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: ToastrService, useValue: toastrMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CreateTaskComponent);
    component = fixture.componentInstance;
    fixture.detectChanges(); // runs ngOnInit
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should submit the form and call createTask', () => {
    component.taskForm.setValue({
      title: 'New Task',
      description: 'This is a test task description',
      dueDate: new Date().toISOString().split('T')[0],
      assignedToId: mockUsers[0].id,
      status: 'ToDo'
    });

    component.submit();

    expect(taskServiceMock.createTask).toHaveBeenCalled();
    expect(toastrMock.success).toHaveBeenCalledWith('Task created successfully');
    expect(routerMock.navigate).toHaveBeenCalledWith(['/tasks']);
  });

  it('should handle API failure on task creation', () => {
    taskServiceMock.createTask.and.returnValue(
      throwError(() => ({ error: { message: 'Failed' } }))
    );

    component.taskForm.setValue({
      title: 'Test Task',
      description: 'This is a test task description.',
      dueDate: new Date().toISOString().split('T')[0],
      assignedToId: '1',
      status: 'ToDo'
    });

    component.submit();

    expect(toastrMock.error).toHaveBeenCalledWith('Failed');
    expect(component.loading).toBeFalse();
  });
});
