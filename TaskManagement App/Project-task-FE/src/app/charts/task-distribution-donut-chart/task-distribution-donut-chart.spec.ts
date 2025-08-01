import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { TaskDistributionDonutChartComponent } from './task-distribution-donut-chart';
import { TaskService } from '../../manager/create-task/task.service';

describe('TaskDistributionDonutChartComponent', () => {
  let component: TaskDistributionDonutChartComponent;
  let fixture: ComponentFixture<TaskDistributionDonutChartComponent>;

  const mockTasks = [
    { status: 'ToDo' },
    { status: 'ToDo' },
    { status: 'InProgress' },
    { status: 'Done' },
    { status: 'Done' },
    { status: 'Done' }
  ];

  const taskServiceMock = {
    getAllTasks: jasmine.createSpy('getAllTasks').and.returnValue(of({ data: mockTasks }))
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskDistributionDonutChartComponent],
      providers: [
        { provide: TaskService, useValue: taskServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TaskDistributionDonutChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should populate chart data correctly', () => {
    expect(component.taskCounts).toEqual({
      ToDo: 2,
      InProgress: 1,
      Done: 3
    });

    expect(component.doughnutChartData.datasets[0].data).toEqual([2, 1, 3]);
  });

  it('should return true from hasChartData() if there is data', () => {
    expect(component.hasChartData()).toBeTrue();
  });
});
