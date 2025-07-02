import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { LucideAngularModule, Eye, CircleX, CirclePlus } from 'lucide-angular';
import { Popup } from '../popup/popup';
import { WorkOutPlanModel } from '../model/WorkOutPlan';
import { WorkOutPlanService } from '../service/workout-plan.service';
import { debounceTime, Subject } from 'rxjs';
import { Router } from '@angular/router';
import { TokenService } from '../service/token.service';
import { UserService } from '../service/user.service';
import { UserPlanService } from '../service/user-plan.service';

export function dateRangeValidator(
  group: AbstractControl
): ValidationErrors | null {
  const startDateValue = group.get('startDate')?.value;
  const endDateValue = group.get('endDate')?.value;

  if (!startDateValue || !endDateValue) return null;

  const startDate = new Date(startDateValue);
  const endDate = new Date(endDateValue);

  const today = new Date();
  today.setHours(0, 0, 0, 0);

  if (startDate < today || endDate < today) {
    return { dateInPast: true };
  }

  if (startDate > endDate) {
    return { dateOrderInvalid: true };
  }

  return null;
}

@Component({
  selector: 'app-plans',
  imports: [
    ReactiveFormsModule,
    CommonModule,
    FormsModule,
    LucideAngularModule,
    Popup,
  ],
  templateUrl: './plans.html',
  styleUrl: './plans.css',
})
export class Plans {
  @ViewChild(Popup) toast!: Popup;

  viewIcon = Eye;
  addIcon = CirclePlus;
  cancelIcon = CircleX;

  showAddNewPlanModal = false;
  showAddNewPlanEroll = false;

  AllPlans: WorkOutPlanModel[] = [];
  filteredPlans: WorkOutPlanModel[] = [];

  searchTerm: string = '';
  sortBy: string = 'name';
  sortOrder: string = 'asc';
  currentPage: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  loggedInUserRole: string | null = '';
  loggedInUserId: string | null = '';
  searchSubject = new Subject<string>();

  clientsOptions: any[] = [];
  plansOptions: any[] = [];

  constructor(
    private workOutPlanService: WorkOutPlanService,
    private route: Router,
    private tokenService: TokenService,
    private userService: UserService,
    private userPlanService: UserPlanService
  ) {}

  ngOnInit() {
    this.loggedInUserRole = this.tokenService.getRole();
    this.loggedInUserId = this.tokenService.getUserId();
    if (this.loggedInUserRole === 'Coach') {
      this.getPlansByCoach();
    } else if (this.loggedInUserRole === 'Admin') {
      this.getAllPlans();
    } else {
      this.getPlansByUserId();
    }
    this.searchSubject.pipe(debounceTime(300)).subscribe(() => {
      this.getAllPlans();
    });
    this.getPlansOptions();
    this.getClientsOptions();
  }

  newPlanForm = new FormGroup(
    {
      planName: new FormControl('', [Validators.required]),
      planDescription: new FormControl('', [Validators.required]),
      startDate: new FormControl('', [Validators.required]),
      endDate: new FormControl('', [Validators.required]),
    },
    {
      validators: dateRangeValidator,
    }
  );

  newPlanEnrollForm = new FormGroup({
    clientId: new FormControl('', [Validators.required]),
    planId: new FormControl('', [Validators.required]),
  });

  get clientId() {
    return this.newPlanEnrollForm.get('clientId');
  }
  get planId() {
    return this.newPlanEnrollForm.get('planId');
  }

  get planName() {
    return this.newPlanForm.get('planName');
  }

  get planDescription() {
    return this.newPlanForm.get('planDescription');
  }

  get startDate() {
    return this.newPlanForm.get('startDate');
  }

  get endDate() {
    return this.newPlanForm.get('endDate');
  }

  getAllPlans() {
    const params = {
      search: this.searchTerm,
      sortBy: this.sortBy,
      sortOrder: this.sortOrder,
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
    };

    this.workOutPlanService.getWorkOutPlansByPagination(params).subscribe({
      next: (response) => {
        this.AllPlans = response.data;
        this.totalCount = response.totalCount;
        this.filteredPlans = [...this.AllPlans];
      },
      error: (error) => {
        console.error('Error fetching plans:', error);
      },
    });
  }

  getPlansByCoach() {
    const coachId = this.tokenService.getUserId();
    if (coachId) {
      this.workOutPlanService.getWorkOutPlanByCoachId(coachId).subscribe({
        next: (response) => {
          const uniquePlans = new Map<string, any>();

          response.forEach((item: any) => {
            uniquePlans.set(item.id, {
              id: item.id,
              name: item.planName,
              description: item.planDescription,
              startDate: item.startDate,
              endDate: item.endDate,
            });
          });
          this.AllPlans = Array.from(uniquePlans.values());
          this.filteredPlans = [...this.AllPlans];
        },
        error: (error) => {
          console.error('Error fetching plans by coach:', error);
        },
      });
    } else {
      console.warn('No coach ID found in token');
    }
  }

  getPlansByUserId() {
    if (this.loggedInUserId === null) {
      this.toast.display('Invalid user ID', 'error');
      return;
    }
    this.workOutPlanService
      .getWorkOutPlanByUserId(this.loggedInUserId)
      .subscribe({
        next: (response) => {
          this.AllPlans = response.map((item: any) => ({
            id: item.id,
            name: item.planName,
            description: item.planDescription,
            startDate: item.startDate,
            endDate: item.endDate,
          }));
          this.filteredPlans = [...this.AllPlans];
          console.log('Plans fetched by user ID:', this.filteredPlans);
        },
        error: (error) => {
          console.error('Error fetching plans by user ID:', error);
        },
      });
  }

  getPlansOptions() {
    this.workOutPlanService.getWorkOutPlans().subscribe({
      next: (response) => {
        this.plansOptions = response;
      },
      error: (error) => {
        console.error('Error fetching plans:', error);
        // this.toast.display('Failed to fetch plans', 'error');
      },
    });
  }

  getClientsOptions() {
    this.userService.getUserOptions().subscribe({
      next: (response) => {
        const users = response?.data?.$values || [];

        this.clientsOptions = users.filter(
          (user: any) => user?.role?.toLowerCase() === 'user' && user?.isActive
        );
      },
      error: (error) => {
        console.error('Error fetching clients:', error);
        // this.toast.display('Failed to fetch clients', 'error');
      },
    });
  }

  onSearch() {
    this.searchTerm = this.searchTerm.trim().toLowerCase();
    this.currentPage = 1;
    this.searchSubject.next(this.searchTerm);
  }

  onSubmitNewPlan() {
    this.currentPage = 1;
    this.getAllPlans();
  }

  addNewPlan() {
    if (this.newPlanForm.invalid) {
      this.newPlanForm.markAllAsTouched();

      const errors = this.newPlanForm.errors;

      if (errors?.['dateInPast']) {
        this.showAddNewPlanModal = false;
        this.toast.display(
          'Please enter dates that are not in the past.',
          'error'
        );
        this.newPlanForm.reset();
      } else if (errors?.['dateOrderInvalid']) {
        this.showAddNewPlanModal = false;

        this.toast.display(
          'Start date must be before or same as end date.',
          'error'
        );
        this.newPlanForm.reset();
      } else {
        this.showAddNewPlanModal = false;

        this.toast.display(
          'Please fill in all required fields correctly.',
          'error'
        );
        this.newPlanForm.reset();
      }
      return;
    }

    const rawStartDate = this.startDate?.value;
    const rawEndDate = this.endDate?.value;
    const utcStartDate = new Date(rawStartDate + 'T00:00:00Z');
    const utcEndDate = new Date(rawEndDate + 'T00:00:00Z');

    const newPlan = {
      id: '',
      name: this.planName?.value || '',
      description: this.planDescription?.value || '',
      startDate: utcStartDate,
      endDate: utcEndDate,
    };

    this.workOutPlanService.AddNewWorkOutPlan(newPlan).subscribe({
      next: (response) => {
        this.showAddNewPlanModal = false;

        this.toast.display('Plan added successfully', 'success');
        this.newPlanForm.reset();
        this.getAllPlans();
      },
      error: (error) => {
        this.showAddNewPlanModal = false;

        console.error('Error adding plan:', error);
        this.toast.display('Failed to add plan', 'error');
      },
    });
  }

  addNewPlanEnroll() {
    if (this.newPlanEnrollForm.invalid) {
      this.newPlanEnrollForm.markAllAsTouched();
      this.toast.display(
        'Please fill in all required fields correctly.',
        'error'
      );
      return;
    }
    const clientId = this.clientId?.value;
    const planId = this.planId?.value;

    if (!clientId || !planId) {
      this.toast.display('Please select both client and plan', 'error');
      return;
    }
    this.userPlanService.addUserPlan(clientId, planId).subscribe({
      next: (response) => {
        this.showAddNewPlanEroll = false;
        this.toast.display('Enrollment added successfully', 'success');
        this.newPlanEnrollForm.reset();
        this.getAllPlans();
      },
      error: (error) => {
        console.error('Error adding enrollment:', error);
        this.toast.display('Failed to add enrollment', 'error');
      },
    });
  }

  onSortChange(sortBy: string) {
    this.sortBy = sortBy;
    this.getAllPlans();
  }

  onPageChange(page: number) {
    this.currentPage = page;
    this.getAllPlans();
  }

  onSortOrderChange(order: string) {
    this.sortOrder = order;
    this.getAllPlans();
  }

  viewPlan(id: any) {
    this.route.navigate(['/manage-plan/plan-details', id]);
  }
}
