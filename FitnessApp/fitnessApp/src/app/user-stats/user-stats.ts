import { Component, ViewChild } from '@angular/core';
import { Popup } from '../popup/popup';
import {
  LucideAngularModule,
  User,
  MailCheck,
  IdCard,
  ShieldCheck,
  Dumbbell,
  CalendarDays,
  CirclePlus,
  CircleX,
  Search,
  View,
} from 'lucide-angular';
import { WeightAnalysis } from '../charts/weight-analysis/weight-analysis';
import { FatAnalysis } from '../charts/fat-analysis/fat-analysis';
import { WaterAnalysis } from '../charts/water-analysis/water-analysis';
import { CalroiesAnalysis } from '../charts/calroies-analysis/calroies-analysis';
import { DataService } from '../service/data.service';
import { CommonModule } from '@angular/common';
import { UserStatsService } from '../service/userstats.service';
import { TokenService } from '../service/token.service';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { WorkoutTaskService } from '../service/workouttask.service';

@Component({
  selector: 'app-user-stats',
  imports: [
    Popup,
    LucideAngularModule,
    WeightAnalysis,
    FatAnalysis,
    WaterAnalysis,
    CalroiesAnalysis,
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
  ],
  templateUrl: './user-stats.html',
  styleUrl: './user-stats.css',
})
export class UserStats {
  @ViewChild(Popup) toast!: Popup;

  userIcon = User;
  mailIcon = MailCheck;
  userIdIcon = IdCard;
  activeIcon = ShieldCheck;
  dumbbellIcon = Dumbbell;
  DateIcon = CalendarDays;
  addIcon = CirclePlus;
  cancelIcon = CircleX;
  searchIcon = Search;

  loggedInUserRole: any = null;

  constructor(
    private dataService: DataService,
    private userStatsService: UserStatsService,
    private tokenService: TokenService,
    private workoutTaskService: WorkoutTaskService
  ) {}

  showAddWorkOutTask: boolean = false;
  showEditTaskModal: boolean = false;
  UserData: any = null;
  allTasks: any[] = [];

  searchTerm: string = '';

  clientId: string | null = null;
  workoutPlanId: string | null = null;
  selectedUser: any = null;
  taskList: any[] = [];

  pageSize = 5;
  currentPage = 1;

  workOutTaskForm = new FormGroup({
    userId: new FormControl('', [Validators.required]),
    planId: new FormControl('', [Validators.required]),
    exerciseName: new FormControl('', [
      Validators.required,
      Validators.minLength(10),
    ]),
    exerciseDescription: new FormControl('', [
      Validators.required,
      Validators.minLength(10),
    ]),
    noofReps: new FormControl('', [Validators.required, Validators.min(1)]),
    noofSets: new FormControl('', [Validators.required, Validators.min(1)]),
    weightlifting: new FormControl('', [
      Validators.required,
      Validators.min(5),
    ]),
    scheduledDate: new FormControl('', [Validators.required]),
  });

  get userId() {
    return this.workOutTaskForm.get('userId');
  }

  get planId() {
    return this.workOutTaskForm.get('planId');
  }

  get exerciseName() {
    return this.workOutTaskForm.get('exerciseName');
  }

  get exerciseDescription() {
    return this.workOutTaskForm.get('exerciseDescription');
  }

  get noofReps() {
    return this.workOutTaskForm.get('noofReps');
  }

  get noofSets() {
    return this.workOutTaskForm.get('noofSets');
  }

  get weightlifting() {
    return this.workOutTaskForm.get('weightlifting');
  }

  get scheduledDate() {
    return this.workOutTaskForm.get('scheduledDate');
  }

  ngOnInit() {
    this.dataService.selectedUser$.subscribe((user) => {
      if (user) {
        this.UserData = user;
        this.clientId = this.UserData?.clientId || null;
        this.workoutPlanId = this.UserData?.id || null;
        localStorage.setItem('userId', this.clientId || '');
        localStorage.setItem('workOutPlanId', this.workoutPlanId || '');
        this.selectedUser = user;
        this.loggedInUserRole = this.tokenService.getRole();
        this.getWorkOutStats();
        this.getAllWorkoutTasks();
      }
    });
  }

  getWorkOutStats() {
    if (!this.clientId || !this.workoutPlanId) {
      console.warn('Missing userId or workoutPlanId');
      return;
    }

    this.userStatsService
      .getWorkOutStats(this.clientId, this.workoutPlanId)
      .subscribe({
        next: (response) => {
          console.log('Workout Stats:');
        },
        error: (error) => {
          console.error('Error fetching workout stats:', error);
        },
      });
  }

  addTask() {
    if (this.workOutTaskForm.invalid) {
      this.workOutTaskForm.markAllAsTouched();
      return;
    }

    const formData = {
      userId: this.workOutTaskForm.value.userId,
      coachId: this.selectedUser?.coachId || '',
      planId: this.workOutTaskForm.value.planId,
      exerciseName: this.workOutTaskForm.value.exerciseName,
      description: this.workOutTaskForm.value.exerciseDescription,
      reps: this.workOutTaskForm.value.noofReps,
      sets: this.workOutTaskForm.value.noofSets,
      weight: this.workOutTaskForm.value.weightlifting,
      scheduledDate: new Date(
        this.workOutTaskForm.value.scheduledDate + 'T00:00:00Z'
      ),
    };

    this.workoutTaskService.addWorkoutTask(formData).subscribe({
      next: (response) => {
        this.showAddWorkOutTask = false;
        this.toast.display('Workout task added successfully!', 'success');
        this.workOutTaskForm.reset();
        this.getWorkOutStats();
      },
      error: (error) => {
        console.error('Error adding workout task:', error);
      },
    });
  }

  getAllWorkoutTasks() {
    this.workoutTaskService
      .getAllWorkoutTasks(this.selectedUser.clientId, this.selectedUser.id)
      .subscribe({
        next: (response) => {
          this.allTasks = (response.data || []).sort(
            (a: { isCompleted: any }, b: { isCompleted: any }) => {
              if (a.isCompleted === b.isCompleted) return 0;
              return a.isCompleted ? 1 : -1;
            }
          );
          this.taskList = [...this.allTasks];

          if (this.taskList.length === 0) {
            this.toast.display('No workout tasks found for this user.', 'info');
          }
        },
        error: (error) => {
          this.toast.display('Error fetching workout tasks.', 'error');
          console.error('Error fetching all workout tasks:', error);
        },
      });
  }

  openEditModal(list: any) {
    const formdata = {
      id: list.id,
      isCompleted: true,
      completedDate: new Date().toISOString(),
    };

    this.workoutTaskService.updateWorkoutTask(formdata, formdata.id).subscribe({
      next: (response) => {
        this.toast.display('Workout task updated successfully!', 'success');
        this.getAllWorkoutTasks();
      },
      error: (error) => {
        console.error('Error updating workout task:', error);
        this.toast.display('Error updating workout task.', 'error');
      },
    });
  }

  openViewModal() {}

  get pagedTasks() {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    return this.taskList.slice(startIndex, startIndex + this.pageSize);
  }

  get totalPages() {
    return Math.ceil(this.taskList.length / this.pageSize);
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  onSearchInput(searchTerm: string) {
    if (searchTerm && searchTerm.trim() !== '') {
      const lowerSearch = searchTerm.toLowerCase();
      this.taskList = this.allTasks.filter((task) => {
        return (
          (task.exerciseName &&
            task.exerciseName.toLowerCase().includes(lowerSearch)) ||
          (task.description &&
            task.description.toLowerCase().includes(lowerSearch)) ||
          (task.scheduledDate &&
            String(task.scheduledDate).toLowerCase().includes(lowerSearch))
        );
      });
    } else {
      this.taskList = [...this.allTasks];
    }

    this.currentPage = 1;
  }

  ngOnDestroy() {
    this.dataService.clearUser();
  }
}
