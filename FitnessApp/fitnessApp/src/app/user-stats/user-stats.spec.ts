import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UserStats } from './user-stats';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { of, throwError, Subject } from 'rxjs';
import { Popup } from '../popup/popup';
import { DataService } from '../service/data.service';
import { UserStatsService } from '../service/userstats.service';
import { TokenService } from '../service/token.service';
import { WorkoutTaskService } from '../service/workouttask.service';

describe('UserStats', () => {
  let component: UserStats;
  let fixture: ComponentFixture<UserStats>;

  // Spies
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let userStatsServiceSpy: jasmine.SpyObj<UserStatsService>;
  let tokenServiceSpy: jasmine.SpyObj<TokenService>;
  let workoutTaskServiceSpy: jasmine.SpyObj<WorkoutTaskService>;

  const mockUser = {
    clientId: 'client1',
    id: 'plan1',
    coachId: 'coach1'
  };

  beforeEach(async () => {
    // Spies for services
    dataServiceSpy = jasmine.createSpyObj('DataService', ['clearUser'], {
      selectedUser$: new Subject<any>()
    });

    userStatsServiceSpy = jasmine.createSpyObj('UserStatsService', ['getWorkOutStats']);
    tokenServiceSpy = jasmine.createSpyObj('TokenService', ['getRole']);
    workoutTaskServiceSpy = jasmine.createSpyObj('WorkoutTaskService', [
      'addWorkoutTask',
      'getAllWorkoutTasks',
      'updateWorkoutTask'
    ]);

    await TestBed.configureTestingModule({
      imports: [UserStats, ReactiveFormsModule, FormsModule, CommonModule],
      providers: [
        { provide: DataService, useValue: dataServiceSpy },
        { provide: UserStatsService, useValue: userStatsServiceSpy },
        { provide: TokenService, useValue: tokenServiceSpy },
        { provide: WorkoutTaskService, useValue: workoutTaskServiceSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(UserStats);
    component = fixture.componentInstance;
    component.toast = jasmine.createSpyObj('Popup', ['display']);
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('addTask', () => {
    it('should mark form as touched if invalid', () => {
      component.workOutTaskForm.reset();
      component.addTask();
      expect(component.workOutTaskForm.touched).toBeTrue();
    });

    it('should call addWorkoutTask and reset form on success', () => {
      component.selectedUser = mockUser;
      component.workOutTaskForm.setValue({
        userId: 'client1',
        planId: 'plan1',
        exerciseName: 'Test Exercise Name',
        exerciseDescription: 'Description of exercise',
        noofReps: '5',
        noofSets: '3',
        weightlifting: '10',
        scheduledDate: '2025-07-01'
      });

      workoutTaskServiceSpy.addWorkoutTask.and.returnValue(of({}));
      spyOn(component, 'getWorkOutStats');

      component.addTask();

      expect(workoutTaskServiceSpy.addWorkoutTask).toHaveBeenCalled();
      expect(component.toast.display).toHaveBeenCalledWith('Workout task added successfully!', 'success');
      expect(component.getWorkOutStats).toHaveBeenCalled();
      expect(component.workOutTaskForm.pristine).toBeTrue();
      expect(component.showAddWorkOutTask).toBeFalse();
    });

    it('should handle error when adding workout task', () => {
      component.selectedUser = mockUser;
      component.workOutTaskForm.setValue({
        userId: 'client1',
        planId: 'plan1',
        exerciseName: 'Test Exercise Name',
        exerciseDescription: 'Description of exercise',
        noofReps: '5',
        noofSets: '3',
        weightlifting: '10',
        scheduledDate: '2025-07-01'
      });

      workoutTaskServiceSpy.addWorkoutTask.and.returnValue(
        throwError(() => new Error('Error'))
      );

      component.addTask();

      expect(component.toast.display).not.toHaveBeenCalledWith('Workout task added successfully!', 'success');
    });
  });

  describe('getAllWorkoutTasks', () => {
    it('should fetch and sort tasks', () => {
      component.selectedUser = mockUser;
      const tasks = [
        { id: '1', isCompleted: false },
        { id: '2', isCompleted: true },
      ];

      workoutTaskServiceSpy.getAllWorkoutTasks.and.returnValue(of({ data: tasks }));

      component.getAllWorkoutTasks();

      expect(component.allTasks.length).toBe(2);
      expect(component.taskList[0].isCompleted).toBe(false);
    });

    it('should display info if no tasks found', () => {
      component.selectedUser = mockUser;
      workoutTaskServiceSpy.getAllWorkoutTasks.and.returnValue(of({ data: [] }));

      component.getAllWorkoutTasks();

      expect(component.toast.display).toHaveBeenCalledWith(
        'No workout tasks found for this user.',
        'info'
      );
    });

    it('should handle error when fetching tasks', () => {
      component.selectedUser = mockUser;
      workoutTaskServiceSpy.getAllWorkoutTasks.and.returnValue(
        throwError(() => new Error('Error'))
      );

      component.getAllWorkoutTasks();

      expect(component.toast.display).toHaveBeenCalledWith(
        'Error fetching workout tasks.',
        'error'
      );
    });
  });

  describe('openEditModal', () => {
    it('should update workout task and refresh', () => {
      const task = { id: 'task1' };
      workoutTaskServiceSpy.updateWorkoutTask.and.returnValue(of({}));
      spyOn(component, 'getAllWorkoutTasks');

      component.openEditModal(task);

      expect(workoutTaskServiceSpy.updateWorkoutTask).toHaveBeenCalledWith(
        jasmine.objectContaining({ id: 'task1', isCompleted: true }),
        'task1'
      );
      expect(component.toast.display).toHaveBeenCalledWith(
        'Workout task updated successfully!',
        'success'
      );
      expect(component.getAllWorkoutTasks).toHaveBeenCalled();
    });

    it('should handle error when updating workout task', () => {
      const task = { id: 'task1' };
      workoutTaskServiceSpy.updateWorkoutTask.and.returnValue(
        throwError(() => new Error('Error'))
      );

      component.openEditModal(task);

      expect(component.toast.display).toHaveBeenCalledWith(
        'Error updating workout task.',
        'error'
      );
    });
  });

  describe('pagination', () => {
    beforeEach(() => {
      component.taskList = Array.from({ length: 12 }, (_, i) => ({ id: `${i}` }));
    });

    it('should compute totalPages correctly', () => {
      expect(component.totalPages).toBe(3);
    });

    it('should go to valid page', () => {
      component.goToPage(2);
      expect(component.currentPage).toBe(2);
    });

    it('should not go to invalid page', () => {
      component.goToPage(0);
      expect(component.currentPage).toBe(1);
    });
  });

  describe('onSearchInput', () => {
    beforeEach(() => {
      component.allTasks = [
        { exerciseName: 'Bench Press', description: 'Chest exercise', scheduledDate: '2025-07-01' },
        { exerciseName: 'Squat', description: 'Leg exercise', scheduledDate: '2025-07-02' }
      ];
    });

    it('should filter tasks by search term', () => {
      component.onSearchInput('bench');
      expect(component.taskList.length).toBe(1);
    });

    it('should reset filter when search term is empty', () => {
      component.onSearchInput('');
      expect(component.taskList.length).toBe(2);
    });
  });

  describe('ngOnDestroy', () => {
    it('should clear selected user', () => {
      component.ngOnDestroy();
      expect(dataServiceSpy.clearUser).toHaveBeenCalled();
    });
  });
});
