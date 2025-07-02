import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ManageCoach } from './manage-coach';
import { of, throwError } from 'rxjs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Popup } from '../popup/popup';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { ManageCoachService } from '../service/manage-coach.service';
import { UserService } from '../service/user.service';
import { TokenService } from '../service/token.service';

describe('ManageCoach', () => {
  let component: ManageCoach;
  let fixture: ComponentFixture<ManageCoach>;
  let coachServiceSpy: jasmine.SpyObj<any>;
  let userServiceSpy: jasmine.SpyObj<any>;
  let tokenServiceSpy: jasmine.SpyObj<any>;

  beforeEach(async () => {
    coachServiceSpy = jasmine.createSpyObj('ManageCoachService', [
      'getCoachOnlyMapping',
      'getAllCoaches',
      'addCoachMapping',
      'deleteCoachMapping',
    ]);

    userServiceSpy = jasmine.createSpyObj('UserService', ['getAllUsers']);
    tokenServiceSpy = jasmine.createSpyObj('TokenService', [
      'getRole',
      'getUserId',
    ]);

    await TestBed.configureTestingModule({
      imports: [
        ManageCoach,
        FormsModule,
        ReactiveFormsModule,
        CommonModule,
        LucideAngularModule,
      ],
      providers: [
        { provide: ManageCoachService, useValue: coachServiceSpy },
        { provide: UserService, useValue: userServiceSpy },
        { provide: TokenService, useValue: tokenServiceSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ManageCoach);
    component = fixture.componentInstance;

    // Mock Popup
    component.toast = jasmine.createSpyObj('Popup', ['display']);
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should call loadAllCoachMapping and loadAllUsers', () => {
      spyOn(component, 'loadAllCoachMapping');
      spyOn(component, 'loadAllUsers');

      component.ngOnInit();

      expect(component.loadAllCoachMapping).toHaveBeenCalled();
      expect(component.loadAllUsers).toHaveBeenCalled();
    });
  });

  describe('loadAllUsers', () => {
    it('should populate coaches and clients arrays', () => {
      userServiceSpy.getAllUsers.and.returnValue(
        of({
          data: {
            $values: [
              { role: 'Coach', name: 'Coach A' },
              { role: 'User', name: 'User A' },
            ],
          },
        })
      );

      component.loadAllUsers();

      expect(component.coaches.length).toBe(1);
      expect(component.clients.length).toBe(1);
    });

    it('should handle error while fetching users', () => {
      userServiceSpy.getAllUsers.and.returnValue(throwError(() => new Error('Error')));
      spyOn(console, 'error');
      component.loadAllUsers();
      expect(console.error).toHaveBeenCalled();
    });
  });

  describe('loadAllCoachMapping', () => {
    it('should load coach-only mapping if user is not Admin', () => {
      tokenServiceSpy.getRole.and.returnValue('Coach');
      tokenServiceSpy.getUserId.and.returnValue('coach1');
      coachServiceSpy.getCoachOnlyMapping.and.returnValue(of([{ coachId: '1' }]));

      component.loadAllCoachMapping();

      expect(coachServiceSpy.getCoachOnlyMapping).toHaveBeenCalledWith('coach1');
    });

    it('should load all coaches if user is Admin', () => {
      tokenServiceSpy.getRole.and.returnValue('Admin');
      coachServiceSpy.getAllCoaches.and.returnValue(of([{ coachId: '1' }]));

      component.loadAllCoachMapping();

      expect(coachServiceSpy.getAllCoaches).toHaveBeenCalled();
    });
  });

  describe('applyFilters', () => {
    it('should filter and paginate coaches', () => {
      component.allCoaches = [
        {
          coachName: 'John Doe',
          coachEmail: 'john@example.com',
          clientName: 'Client A',
          clientEmail: 'client@example.com',
        },
      ];

      component.searchTerm = 'john';
      component.applyFilters();

      expect(component.filteredCoaches.length).toBe(1);
      expect(component.totalPages).toBe(1);
    });
  });

  describe('addMapping', () => {
    it('should display error if coach or client not selected', () => {
      component.addMappingForm.setValue({ coach: null, client: null });
      component.addMapping();
      expect(component.toast.display).toHaveBeenCalledWith(
        'Please select both coach and client.',
        'error'
      );
    });

    it('should call addCoachMapping and reset form on success', () => {
      component.addMappingForm.setValue({ coach: '1', client: '2' });
      coachServiceSpy.addCoachMapping.and.returnValue(of({}));

      spyOn(component, 'loadAllCoachMapping');

      component.addMapping();

      expect(coachServiceSpy.addCoachMapping).toHaveBeenCalledWith('1', '2');
      expect(component.loadAllCoachMapping).toHaveBeenCalled();
      expect(component.addMappingForm.value).toEqual({ coach: null, client: null });
      expect(component.showAddCoachForm).toBeFalse();
      expect(component.toast.display).toHaveBeenCalledWith(
        'Coach-Client mapping added successfully.',
        'success'
      );
    });

    it('should handle error when adding mapping', () => {
      component.addMappingForm.setValue({ coach: '1', client: '2' });
      coachServiceSpy.addCoachMapping.and.returnValue(throwError(() => new Error('Error')));
      component.addMapping();
      expect(component.toast.display).toHaveBeenCalledWith(
        'Failed to add mapping. Please try again.',
        'error'
      );
    });
  });

  describe('deleteMapping', () => {
    it('should display error if no mapping selected', () => {
      component.selectedCoach = null;
      component.deleteMapping();
      expect(component.toast.display).toHaveBeenCalledWith(
        'No mapping selected for deletion.',
        'error'
      );
    });

    it('should call deleteCoachMapping and reload on success', () => {
      component.selectedCoach = { coachId: '1', clientId: '2' };
      coachServiceSpy.deleteCoachMapping.and.returnValue(of({}));
      spyOn(component, 'loadAllCoachMapping');

      component.deleteMapping();

      expect(coachServiceSpy.deleteCoachMapping).toHaveBeenCalledWith('1', '2');
      expect(component.loadAllCoachMapping).toHaveBeenCalled();
      expect(component.showDeleteModal).toBeFalse();
      expect(component.selectedCoach).toBeNull();
      expect(component.toast.display).toHaveBeenCalledWith(
        'Coach-Client mapping deleted successfully.',
        'success'
      );
    });

    it('should handle error when deleting mapping', () => {
      component.selectedCoach = { coachId: '1', clientId: '2' };
      coachServiceSpy.deleteCoachMapping.and.returnValue(
        throwError(() => new Error('Error'))
      );

      component.deleteMapping();

      expect(component.toast.display).toHaveBeenCalledWith(
        'Failed to delete mapping. Please try again.',
        'error'
      );
    });
  });
});
