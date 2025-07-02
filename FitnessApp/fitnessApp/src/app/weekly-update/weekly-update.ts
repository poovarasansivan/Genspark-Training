import { Component, ViewChild } from '@angular/core';
import { Popup } from '../popup/popup';
import {
  LucideAngularModule,
  SearchIcon,
  CirclePlus,
  CircleX,
} from 'lucide-angular';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ProgressService } from '../service/progress.service';
import { TokenService } from '../service/token.service';
import { WorkOutPlanService } from '../service/workout-plan.service';

@Component({
  selector: 'app-weekly-update',
  imports: [
    Popup,
    LucideAngularModule,
    ReactiveFormsModule,
    FormsModule,
    CommonModule,
  ],
  templateUrl: './weekly-update.html',
  styleUrl: './weekly-update.css',
})
export class WeeklyUpdate {
  @ViewChild(Popup) toast!: Popup;

  searchIcon = SearchIcon;
  addIcon = CirclePlus;
  cancelIcon = CircleX;

  searchTerm: string = '';
  selectedImage: File | null = null;

  showAddProgressModal: boolean = false;
  showProgressDetailsModal: boolean = false;
  showEditProgressModal: boolean = false;

  selectedProgress: any = null;
  AllProgress: any[] = [];
  filteredProgress: any[] = [];
  totalProgress: number = 0;
  pageNumbers: number[] = [];

  loggedInUserId: string | null = null;
  loggedInUser: string | null = null;
  loggedInUserRole: string | null = null;

  WorkOutPlansOptions: any[] = [];

  progressFilters = {
    pageNumber: 1,
    pageSize: 10,
  };

  addProgress = new FormGroup({
    name: new FormControl('', [Validators.required]),
    workOutPlan: new FormControl('', [Validators.required]),
    submissionDate: new FormControl('', [Validators.required]),
    bodyWeight: new FormControl('', [Validators.required]),
    fatPercentage: new FormControl('', [Validators.required]),
    muscleMass: new FormControl('', [Validators.required]),
    waterPercentage: new FormControl('', [Validators.required]),
  });

  editProgressForm = new FormGroup({
    editPlanName: new FormControl('', [Validators.required]),
    esubmissionDate: new FormControl('', [Validators.required]),
    ebodyWeight: new FormControl('', [Validators.required]),
    efatPercentage: new FormControl('', [Validators.required]),
    emuscleMass: new FormControl('', [Validators.required]),
    ewaterPercentage: new FormControl('', [Validators.required]),
  });

  get emuscleMass() {
    return this.editProgressForm.get('emuscleMass')!;
  }
  get ewaterPercentage() {
    return this.editProgressForm.get('ewaterPercentage')!;
  }

  get efatPercentage() {
    return this.editProgressForm.get('efatPercentage')!;
  }

  get editPlanName() {
    return this.editProgressForm.get('editPlanName')!;
  }

  get esubmissionDate() {
    return this.editProgressForm.get('esubmissionDate')!;
  }

  get ebodyWeight() {
    return this.editProgressForm.get('ebodyWeight')!;
  }

  get name() {
    return this.addProgress.get('name')!;
  }
  get workOutPlan() {
    return this.addProgress.get('workOutPlan')!;
  }
  get submissionDate() {
    return this.addProgress.get('submissionDate')!;
  }
  get bodyWeight() {
    return this.addProgress.get('bodyWeight')!;
  }
  get fatPercentage() {
    return this.addProgress.get('fatPercentage')!;
  }
  get muscleMass() {
    return this.addProgress.get('muscleMass')!;
  }
  get waterPercentage() {
    return this.addProgress.get('waterPercentage')!;
  }

  constructor(
    private progressService: ProgressService,
    private tokenService: TokenService,
    private workOutPlanoption: WorkOutPlanService
  ) {}

  ngOnInit() {
    this.loggedInUserId = this.tokenService.getUserId();
    this.loggedInUser = this.tokenService.getUsername();
    this.loggedInUserRole = this.tokenService.getRole();

    if (this.loggedInUserRole === 'Coach') {
      this.getProgressByCoachId();
    } else if (this.loggedInUserRole === 'Admin') {
      this.getAllProgressWithPagination();
    } else {
      this.getProgressByClientId();
    }

    this.getWorkOutPlansOptions();
  }

  getWorkOutPlansOptions() {
    this.workOutPlanoption.getWorkOutPlans().subscribe({
      next: (response) => {
        this.WorkOutPlansOptions = response;
      },
      error: (error) => {
        this.toast.display(
          'Error fetching workout plans. Please try again later.',
          'error'
        );
        console.error('Error fetching workout plans:', error);
      },
    });
  }

  getAllProgressWithPagination() {
    this.progressService
      .getAllProgressWithPagination(this.progressFilters)
      .subscribe({
        next: (response: any) => {
          this.AllProgress = response.data;
          this.filteredProgress = response.data;
          this.totalProgress = response.totalCount;
          this.pageNumbers = this.UpdatePageNumbers();
        },
        error: (error) => {
          this.toast.display(
            'Error fetching progress data. Please try again later.',
            'error'
          );
          console.error('Error fetching progress:', error);
        },
      });
  }

  getProgressByCoachId() {
    if (!this.loggedInUserId) {
      this.toast.display('Invalid user ID', 'error');
      return;
    }
    this.progressService.getProgressByCoachId(this.loggedInUserId).subscribe({
      next: (response: any[]) => {
        this.AllProgress = response;
        this.filteredProgress = [...this.AllProgress];
        this.totalProgress = this.filteredProgress.length;
        this.pageNumbers = this.UpdatePageNumbers();
        this.toast.display('Progress fetched successfully', 'success');
      },
      error: (error) => {
        this.toast.display('Failed to fetch progress', 'error');
        console.error('Error fetching progress by coach:', error);
      },
    });
  }

  getProgressByClientId() {
    if (this.loggedInUserId === null) {
      this.toast.display('Invalid user ID', 'error');
      return;
    }
    this.progressService.getProgressByUserId(this.loggedInUserId).subscribe({
      next: (response: any[]) => {
        this.AllProgress = response;
        this.filteredProgress = [...this.AllProgress];
        this.totalProgress = this.filteredProgress.length;
        this.pageNumbers = this.UpdatePageNumbers();
        this.toast.display('Progress fetched successfully', 'success');
      },
      error: (error) => {
        this.toast.display('Failed to fetch progress', 'error');
        console.error('Error fetching progress by client:', error);
      },
    });
  }

  addNewProgress() {
    if (this.addProgress.invalid) {
      this.addProgress.markAllAsTouched();
      this.toast.display('Please fill all required fields.', 'error');
      return;
    }
    const date = new Date(this.addProgress.value.submissionDate + 'T00:00:00Z');
    const progressData = {
      userId: this.addProgress.value.name,
      workOutPlanId: this.addProgress.value.workOutPlan,
      date: date,
      weight: this.addProgress.value.bodyWeight,
      bodyFatPercentage: this.addProgress.value.fatPercentage,
      muscleMass: this.addProgress.value.muscleMass,
      waterPercentage: this.addProgress.value.waterPercentage,
      notes: 'NIL',
    };

    this.progressService.addNewProgress(progressData).subscribe({
      next: (response) => {
        const progressId = response.id;

        if (this.selectedImage) {
          this.progressService
            .addProgressImage(this.selectedImage, progressId)
            .subscribe({
              next: () => {
                this.toast.display(
                  'Progress and image uploaded successfully!',
                  'success'
                );
                this.afterSuccess();
              },
              error: (error) => {
                this.toast.display(
                  'Progress saved, but image upload failed.',
                  'info'
                );
                console.error('Image upload error:', error);
                this.afterSuccess(); // still reset form
              },
            });
        } else {
          this.toast.display('Progress added successfully!', 'success');
          this.afterSuccess();
        }
      },
      error: (error) => {
        this.toast.display(
          'Error adding progress. Please try again later.',
          'error'
        );
        console.error('Progress creation error:', error);
      },
    });
  }

  afterSuccess() {
    this.ngOnInit();
    this.showAddProgressModal = false;
    this.addProgress.reset();
    this.selectedImage = null;
  }

  viewProgressDetails(progress: any) {
    this.showProgressDetailsModal = true;
    this.selectedProgress = progress;
  }

  onSearchInput(searchTerm: string) {}

  onImageSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedImage = input.files[0];
    }
  }

  UpdatePageNumbers(): number[] {
    const pageSize = this.progressFilters.pageSize ?? 10;
    const totalPages = Math.ceil(this.totalProgress / pageSize);
    const currentPage = this.progressFilters.pageNumber ?? 1;
    const pageNumbers: number[] = [];

    if (totalPages <= 5) {
      return Array.from({ length: totalPages }, (_, i) => i + 1);
    }
    pageNumbers.push(1);

    if (currentPage > 3) pageNumbers.push(-1);
    if (currentPage > 2) pageNumbers.push(currentPage - 1);

    const startPage = Math.max(2, currentPage - 1);
    const endPage = Math.min(totalPages - 1, currentPage + 1);

    for (let i = startPage; i <= endPage; i++) {
      pageNumbers.push(i);
    }

    if (currentPage < totalPages - 2) pageNumbers.push(-2);
    pageNumbers.push(totalPages);

    return pageNumbers;
  }

  changePage(page: number) {
    if (
      page < 1 ||
      page > Math.ceil(this.totalProgress / this.progressFilters.pageSize)
    )
      return;

    this.progressFilters.pageNumber = page;
    this.getAllProgressWithPagination();
  }

  editProgressDetails(progress: any) {
    this.selectedProgress = progress;
    this.showEditProgressModal = true;
  }

  updateProgress() {
    const date = new Date(
      this.editProgressForm.value.esubmissionDate + 'T00:00:00Z'
    );

    const progressData = {
      workOutLogId: this.editProgressForm.value.editPlanName,
      date: date,
      weight: this.editProgressForm.value.ebodyWeight || 0,
      bodyFatPercentage: this.editProgressForm.value.efatPercentage ||0,
      muscleMass: this.editProgressForm.value.emuscleMass|| 0,
      waterPercentage: this.editProgressForm.value.ewaterPercentage || 0,
    };
    this.progressService
      .updateProgress(this.selectedProgress.id, progressData)
      .subscribe({
        next: (response) => {
          this.showEditProgressModal = false;
          this.toast.display('Progress updated successfully!', 'success');
          this.ngOnInit();
          this.selectedImage = null;
          this.editProgressForm.reset();
        },
        error: (error) => {
          this.toast.display(
            'Error updating progress. Please try again later.',
            'error'
          );
          console.error('Error updating progress:', error);
        },
      });
  }
}
