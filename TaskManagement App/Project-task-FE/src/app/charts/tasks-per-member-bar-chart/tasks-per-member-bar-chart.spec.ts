import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { TasksPerMemberBarChartComponent } from './tasks-per-member-bar-chart';
import { TaskService } from '../../manager/create-task/task.service';

describe('TasksPerMemberBarChartComponent', () => {
  let component: TasksPerMemberBarChartComponent;
  let fixture: ComponentFixture<TasksPerMemberBarChartComponent>;

  const mockTasks = [
    { assignedToId: '1', assignedToName: 'Alice' },
    { assignedToId: '2', assignedToName: 'Bob' },
    { assignedToId: '1', assignedToName: 'Alice' }
  ];

  const taskServiceMock = {
    getAllTasks: jasmine.createSpy('getAllTasks').and.returnValue(of({ data: mockTasks }))
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TasksPerMemberBarChartComponent],
      providers: [
        { provide: TaskService, useValue: taskServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TasksPerMemberBarChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should populate bar chart data based on task assignments', () => {
    expect(component.barChartData.labels).toEqual(['Alice', 'Bob']);
    expect(component.barChartData.datasets[0].data).toEqual([2, 1]);
  });

  it('should have bar chart data', () => {
    expect(component.hasBarChartData()).toBeTrue();
  });
});
