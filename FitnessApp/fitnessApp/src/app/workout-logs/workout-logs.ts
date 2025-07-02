import { Component, ViewChild } from '@angular/core';
import { Popup } from '../popup/popup';
import { CommonModule } from '@angular/common';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  LucideAngularModule,
  SearchIcon,
  CirclePlus,
  CircleX,
} from 'lucide-angular';
import { WorkLog } from '../model/WorkLog';
import { WorkOutLogService } from '../service/workout-log.service';
import { TokenService } from '../service/token.service';
import { WorkOutPlanService } from '../service/workout-plan.service';

@Component({
  selector: 'app-workout-logs',
  imports: [
    Popup,
    CommonModule,
    FormsModule,
    LucideAngularModule,
    ReactiveFormsModule,
  ],
  templateUrl: './workout-logs.html',
  styleUrl: './workout-logs.css',
})
export class WorkoutLogs {
  @ViewChild(Popup) toast!: Popup;

  searchIcon = SearchIcon;
  searchTerm: string = '';
  addIcon = CirclePlus;
  cancelIcon = CircleX;

  showAddLogModal: boolean = false;
  showViewLogModal: boolean = false;
  showEditLogModal: boolean = false;

  allWorkoutLogs: WorkLog[] = [];
  filteredWorkoutLogs: WorkLog[] = [];
  totalLogs: number = 0;
  pageNumbers: number[] = [];

  plansOptions: any[] = [];

  selectedLog: WorkLog | null = null;

  LogFilter = {
    pageNumber: 1,
    pageSize: 10,
  };

  constructor(
    private workOutLogService: WorkOutLogService,
    private tokenService: TokenService,
    private workOutPlan: WorkOutPlanService
  ) {}

  loggedInUserId: string | null = null;
  loggedInUserName: string | null = null;
  loggedInUserRole: string | null = null;

  loggedInCoachId: string | null = null;

  addWorkLog = new FormGroup({
    name: new FormControl('', [Validators.required]),
    plan: new FormControl('', [Validators.required]),
    workoutType: new FormControl('', [Validators.required]),
    date: new FormControl('', [Validators.required]),
    duration: new FormControl('', [Validators.required]),
    calories: new FormControl('', [Validators.required]),
  });

  editWorkLog = new FormGroup({
    workouttype: new FormControl('', [Validators.required]),
    editplan: new FormControl('', [Validators.required]),
    editdate: new FormControl('', [Validators.required]),
    editduration: new FormControl('', [Validators.required]),
    editcalories: new FormControl('', [Validators.required]),
  });

  get editdate() {
    return this.editWorkLog.get('editdate');
  }
  get editduration() {
    return this.editWorkLog.get('editduration');
  }
  get editcalories() {
    return this.editWorkLog.get('editcalories');
  }
  get editplan() {
    return this.editWorkLog.get('editplan');
  }

  get workouttype() {
    return this.editWorkLog.get('workouttype');
  }

  get name() {
    return this.addWorkLog.get('name');
  }

  get plan() {
    return this.addWorkLog.get('plan');
  }
  get workoutType() {
    return this.addWorkLog.get('workoutType');
  }
  get date() {
    return this.addWorkLog.get('date');
  }
  get duration() {
    return this.addWorkLog.get('duration');
  }
  get calories() {
    return this.addWorkLog.get('calories');
  }

  ngOnInit() {
    this.loggedInUserId = this.tokenService.getUserId();
    this.loggedInCoachId = this.tokenService.getUserId();
    this.loggedInUserName = this.tokenService.getUsername();
    this.loggedInUserRole = this.tokenService.getRole();
    if (this.loggedInUserRole === 'Coach') {
      this.getWorkoutLogsByCoachId();
    } else if (this.loggedInUserRole === 'User') {
      this.getWorkoutLogsByClientId();
    } else {
      this.getAllWorkoutLogs();
    }
    this.getAllPlansOptions();
  }

  getAllWorkoutLogs() {
    this.workOutLogService
      .getAllWorkOutLogsWithPagination(this.LogFilter)
      .subscribe({
        next: (response) => {
          this.allWorkoutLogs = response.data;
          this.filteredWorkoutLogs = this.allWorkoutLogs;
          this.totalLogs = response.totalCount;
          this.pageNumbers = this.updatePageNumbers();
          console.log('Workout logs fetched successfully:', this.totalLogs);
        },
        error: (error) => {
          this.toast.display('Error fetching workout logs', 'error');
          console.error('Error fetching workout logs:', error);
        },
      });
  }

  getWorkoutLogsByCoachId() {
    if (!this.loggedInCoachId) {
      this.toast.display('Invalid coach id', 'error');
      return;
    }
    this.workOutLogService.getWorkOutByCoachId(this.loggedInCoachId).subscribe({
      next: (response) => {
        this.allWorkoutLogs = response;
        this.filteredWorkoutLogs = this.allWorkoutLogs;
        this.totalLogs = this.filteredWorkoutLogs.length;
        this.pageNumbers = this.updatePageNumbers();
        console.log('Workout logs fetched by coach:', this.totalLogs);
      },
      error: (error) => {
        this.toast.display('Failed to fetch workout logs by coach', 'error');
        console.error('Error fetching workout logs by coach:', error);
      },
    });
  }

  getWorkoutLogsByClientId() {
    if (!this.loggedInUserId) {
      this.toast.display('Invalid user id', 'error');
      return;
    }
    this.workOutLogService.getWorkOutByClientId(this.loggedInUserId).subscribe({
      next: (response) => {
        this.allWorkoutLogs = response;
        this.filteredWorkoutLogs = this.allWorkoutLogs;
        this.totalLogs = this.filteredWorkoutLogs.length;
        this.pageNumbers = this.updatePageNumbers();
        console.log('Workout logs fetched by client:', this.totalLogs);
      },
      error: (error) => {
        this.toast.display('Failed to fetch workout logs by client', 'error');
        console.error('Error fetching workout logs by client:', error);
      },
    });
  }

  onSearchInput(searchTerm: string) {
    this.searchTerm = searchTerm.toLowerCase().trim();
    this.filteredWorkoutLogs = this.allWorkoutLogs.filter(
      (log) =>
        log.workOutPlanId.toLowerCase().includes(this.searchTerm) ||
        log.workOutPlanName.toLowerCase().includes(this.searchTerm) ||
        log.userId.toLowerCase().includes(this.searchTerm) ||
        log.userName.toLowerCase().includes(this.searchTerm) ||
        log.type.toLowerCase().includes(this.searchTerm) ||
        log.date.toLowerCase().includes(this.searchTerm) ||
        log.duration.toLowerCase().includes(this.searchTerm)
    );
    this.totalLogs = this.filteredWorkoutLogs.length;
  }

  onClickViewLog(log: WorkLog) {
    this.selectedLog = log;
    this.showViewLogModal = true;
  }

  addNewLog() {
    if (this.addWorkLog.invalid) {
      this.addWorkLog.markAllAsTouched();
      this.toast.display('Please fill all required fields', 'error');
      return;
    }

    console.log('Form Values:', this.addWorkLog.value);
    const utcdate = new Date(this.addWorkLog.value.date + 'T00:00:00Z');
    const newLog = {
      userId: this.addWorkLog.value.name || '',
      workOutPlanId: this.addWorkLog.value.plan || '',
      type: this.addWorkLog.value.workoutType || '',
      date: utcdate || '',
      duration: this.addWorkLog.value.duration || '',
      caloriesBurned: this.addWorkLog.value.calories || 0,
    };

    this.workOutLogService.addNewWorkOutLog(newLog).subscribe({
      next: (response) => {
        this.showAddLogModal = false;
        this.ngOnInit();
        this.toast.display('Workout log added successfully', 'success');
        this.addWorkLog.reset();
      },
      error: (error) => {
        console.error('Error adding workout log:', error);
        console.log(error.message);
        this.toast.display('Failed to add workout log', 'error');
      },
    });
  }

  getAllPlansOptions() {
    this.workOutPlan.getWorkOutPlans().subscribe({
      next: (response) => {
        this.plansOptions = response;
      },
      error: (error) => {
        console.error('Error fetching plans:', error);
        this.toast.display('Failed to fetch plans', 'error');
      },
    });
  }

  updatePageNumbers(): number[] {
    const pageSize = this.LogFilter.pageSize ?? 5;
    const totalPages = Math.ceil(this.totalLogs / pageSize);
    const currentPage = this.LogFilter.pageNumber ?? 1;

    const pages: number[] = [];

    if (totalPages <= 5) {
      return Array.from({ length: totalPages }, (_, i) => i + 1);
    }

    pages.push(1);

    if (currentPage > 3) pages.push(-1);
    if (currentPage > 2) pages.push(currentPage - 1);

    const startPage = Math.max(2, currentPage - 1);
    const endPage = Math.min(totalPages - 1, currentPage + 1);

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    if (currentPage < totalPages - 2) pages.push(-2);
    pages.push(totalPages);

    return pages;
  }

  changePage(page: number) {
    if (page < 1 || page > Math.ceil(this.totalLogs / this.LogFilter.pageSize))
      return;

    this.LogFilter.pageNumber = page;
    this.getAllWorkoutLogs();
  }

  onClickEditLog(log: WorkLog) {
    this.selectedLog = log;
    this.showEditLogModal = true;
  }

  updateLog() {
    // if (this.editWorkLog.invalid) {
    //   this.editWorkLog.markAllAsTouched();
    //   this.toast.display('Please fill all required fields', 'error');
    //   return;
    // }

    console.log('Edit Form Values:', this.editWorkLog.value);
    const utcdate = new Date(this.editWorkLog.value.editdate + 'T00:00:00Z');
    const updatedLog = {
      workOutPlanId: this.editWorkLog.value.editplan || '',
      type: this.editWorkLog.value.workouttype || '',
      date: new Date(this.editdate?.value + 'T00:00:00Z') || '',
      duration: this.editWorkLog.value.editduration || '',
      caloriesBurned: this.editWorkLog.value.editcalories || 0,
    };

    if (this.selectedLog) {
      this.workOutLogService
        .updateWorkOutLog(this.selectedLog.id, updatedLog)
        .subscribe({
          next: (response) => {
            this.showEditLogModal = false;
            this.ngOnInit();
            this.toast.display('Workout log updated successfully', 'success');
            this.selectedLog = null;
            this.editWorkLog.reset();
          },
          error: (error) => {
            console.error('Error updating workout log:', error);
            this.toast.display('Failed to update workout log', 'error');
          },
        });
    }
  }
}
