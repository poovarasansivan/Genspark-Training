import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { BarChart } from './bar-chart';
import { UserService } from '../../service/user.service';

describe('BarChart', () => {
  let component: BarChart;
  let fixture: ComponentFixture<BarChart>;
  let userServiceSpy: jasmine.SpyObj<UserService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('UserService', ['getPlanAnalysis']);
    spy.getPlanAnalysis.and.returnValue(
      of({
        data: {
          $values: [
            {
              workOutPlanId: '1',
              workOutPlanName: 'Plan A',
              coachId: 'coach1',
              isCompleted: 'Completed',
            },
            {
              workOutPlanId: '1',
              workOutPlanName: 'Plan A',
              coachId: 'coach2',
              isCompleted: 'Not Completed',
            },
            {
              workOutPlanId: '2',
              workOutPlanName: 'Plan B',
              coachId: 'coach1',
              isCompleted: 'Completed',
            },
          ],
        },
      })
    );

    await TestBed.configureTestingModule({
      imports: [BarChart],
      providers: [{ provide: UserService, useValue: spy }],
    }).compileComponents();

    fixture = TestBed.createComponent(BarChart);
    component = fixture.componentInstance;
    userServiceSpy = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });


  it('should load chart data and populate series and categories', () => {
    component.loadChartData();

    expect(userServiceSpy.getPlanAnalysis).toHaveBeenCalled();

    const series = component.chartOptions.series;
    const categories = component.chartOptions.xaxis.categories;

    expect(categories).toEqual(['Plan A', 'Plan B']);

    expect(series).toEqual([
      { name: 'Users', data: [2, 1] },       
      { name: 'Coaches', data: [2, 1] },        
      { name: 'Completed Users', data: [1, 1] },
    ]);
  });
});
