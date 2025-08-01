import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TaskEditModalComponent } from './task-edit-modal';
import { TaskItemResponseDto, TaskState } from '../create-task/task.types';
import { of } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../auth/auth.service';
import { TaskService } from '../create-task/task.service';
import { UserService } from '../Create-User/user.service';

describe('TaskEditModalComponent', () => {
  let component: TaskEditModalComponent;
  let fixture: ComponentFixture<TaskEditModalComponent>;

  const mockTask: TaskItemResponseDto = {
  id: '1',
  title: 'Test Task',
  description: 'Test Desc',
  status: TaskState.ToDo,
  dueDate: new Date().toISOString().substring(0, 10),
  createdById: 'manager123',
  createdAt: new Date().toISOString(),
  assignedToId: 'user123',
  assignedToName: 'John Doe'
};

  const authServiceMock = {
    getUser: () => ({ id: 'manager123', role: 'Manager' }) // or 'TeamMember' to test team member logic
  };

  const toastrMock = jasmine.createSpyObj('ToastrService', ['success', 'error']);
  const userServiceMock = jasmine.createSpyObj('UserService', ['getAllUsers']);
  userServiceMock.getAllUsers.and.returnValue(of({ data: [{ id: 'user123', fullName: 'John Doe', role: 'TeamMember' }] }));

  const taskServiceMock = jasmine.createSpyObj('TaskService', ['updateTask']);
  taskServiceMock.updateTask.and.returnValue(of({}));

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskEditModalComponent],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: ToastrService, useValue: toastrMock },
        { provide: TaskService, useValue: taskServiceMock },
        { provide: UserService, useValue: userServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TaskEditModalComponent);
    component = fixture.componentInstance;
    component.task = mockTask; // Set required input before init
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form for manager with all controls', () => {
    expect(component.form.contains('title')).toBeTrue();
    expect(component.form.contains('description')).toBeTrue();
    expect(component.form.contains('dueDate')).toBeTrue();
    expect(component.form.contains('assignedToId')).toBeTrue();
    expect(component.form.contains('status')).toBeTrue();
  });

  it('should submit task update successfully', () => {
    component.submit();

    expect(taskServiceMock.updateTask).toHaveBeenCalled();
    expect(toastrMock.success).toHaveBeenCalledWith('Task updated successfully');
  });
});
