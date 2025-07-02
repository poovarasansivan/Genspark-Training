import { Component, ViewChild } from '@angular/core';
import { Popup } from '../popup/popup';
import { LucideAngularModule, Search, CircleX } from 'lucide-angular';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { WorkOutPlanService } from '../service/workout-plan.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { DataService } from '../service/data.service';
import { TokenService } from '../service/token.service';

@Component({
  selector: 'app-plan-details',
  imports: [
    Popup,
    LucideAngularModule,
    FormsModule,
    CommonModule,
    ReactiveFormsModule,
  ],
  templateUrl: './plan-details.html',
  styleUrl: './plan-details.css',
})
export class PlanDetails {
  @ViewChild(Popup) toast!: Popup;
  searchTerm: string = '';

  cancelIcon = CircleX;
  searchIcon = Search;
  AllPlanDetails: any[] = [];
  filteredPlanDetails: any[] = [];
  showEditPlanModal: boolean = false;

  selectedUserAndPlan: any | null = null;

  loggedInUser: string | null = null;
  loggedInRole: string | null = null;

  pageSize = 10;
  currentPage = 1;
  pageNumbers: number[] = [];

  constructor(
    private workOutPlanService: WorkOutPlanService,
    private route: ActivatedRoute,
    private router: Router,
    private dataService: DataService,
    private tokenService: TokenService
  ) {}

  ngOnInit() {
    this.loggedInRole = this.tokenService.getRole();
    this.loggedInUser = this.tokenService.getUserId();

    if (this.loggedInRole === 'Admin') {
      this.getAllPlanDetails();
    } else if (this.loggedInRole === 'Coach') {
      this.getPlansByCoachId();
    }else{
      this.getPlansByUserId();
    }
  }

  editPlanForm = new FormGroup({
    isCompleted: new FormControl('', Validators.required),
  });

  get isCompleted() {
    return this.editPlanForm.get('isCompleted');
  }

  getAllPlanDetails() {
    const id = this.route.snapshot.paramMap.get('id');
    this.workOutPlanService.getGroupedWorkOutPlans(id).subscribe(
      (response: any) => {
        this.AllPlanDetails = response;
        this.filteredPlanDetails = [...this.AllPlanDetails];
        this.currentPage = 1;
        this.updatePageNumbers();
        this.toast.display('Plan details fetched successfully', 'success');
      },
      (error: any) => {
        this.toast.display('Failed to fetch plan details', 'error');
        console.error('Error fetching plan details:', error);
      }
    );
  }

  getPlansByCoachId() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!this.loggedInUser) {
      this.toast.display('Invalid coach id', 'error');
      return;
    }

    this.workOutPlanService
      .getWorkOutPlanByCoachId(this.loggedInUser)
      .subscribe({
        next: (response: any[]) => {
          this.AllPlanDetails = response;

          this.filteredPlanDetails = this.AllPlanDetails.filter(
            (plan) => plan.id === id
          );

          if (this.filteredPlanDetails.length === 0) {
            this.toast.display('No matching plan found for this ID', 'info');
          }
        },
        error: (error) => {
          console.error('Error fetching plans by coach:', error);
          this.toast.display('Failed to fetch plans', 'error');
        },
      });
  }

  getPlansByUserId() {
    if (!this.loggedInUser) {
      this.toast.display('Invalid user ID', 'error');
      return;
    }

    this.workOutPlanService
      .getWorkOutPlanByUserId(this.loggedInUser)
      .subscribe({
        next: (response: any[]) => {
          this.AllPlanDetails = response;
          this.filteredPlanDetails = [...this.AllPlanDetails];
        },
        error: (error) => {
          console.error('Error fetching plans by user ID:', error);
          this.toast.display('Failed to fetch plans', 'error');
        },
      });
  }

  onSearchInput(search: string) {
    if (search) {
      const lowerSearch = search.toLowerCase();
      this.filteredPlanDetails = this.AllPlanDetails.filter(
        (plan: any) =>
          plan.planName.toLowerCase().includes(lowerSearch) ||
          plan.planDescription.toLowerCase().includes(lowerSearch) ||
          plan.startDate.toLowerCase().includes(lowerSearch) ||
          plan.endDate.toLowerCase().includes(lowerSearch) ||
          plan.coachName.toLowerCase().includes(lowerSearch) ||
          plan.clientName.toLowerCase().includes(lowerSearch) ||
          plan.isCompleted.toLowerCase().includes(lowerSearch)
      );
    } else {
      this.filteredPlanDetails = this.AllPlanDetails;
    }

    this.currentPage = 1;
    this.updatePageNumbers();
  }

  get paginatedPlanDetails(): any[] {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    return this.filteredPlanDetails.slice(
      startIndex,
      startIndex + this.pageSize
    );
  }

  updatePageNumbers() {
    const totalPages = Math.ceil(
      this.filteredPlanDetails.length / this.pageSize
    );
    this.pageNumbers = [];

    if (totalPages <= 7) {
      this.pageNumbers = Array.from({ length: totalPages }, (_, i) => i + 1);
    } else {
      const visiblePages = [];

      if (this.currentPage <= 4) {
        visiblePages.push(1, 2, 3, 4, 5, -1, totalPages);
      } else if (this.currentPage >= totalPages - 3) {
        visiblePages.push(
          1,
          -1,
          totalPages - 4,
          totalPages - 3,
          totalPages - 2,
          totalPages - 1,
          totalPages
        );
      } else {
        visiblePages.push(
          1,
          -1,
          this.currentPage - 1,
          this.currentPage,
          this.currentPage + 1,
          -2,
          totalPages
        );
      }

      this.pageNumbers = visiblePages;
    }
  }

  changePage(page: number) {
    this.currentPage = page;
    this.updatePageNumbers();
  }

  openEditModal(plan: any) {
    this.selectedUserAndPlan = plan;
    this.showEditPlanModal = true;
  }

  onEditPlanSubmit() {
    var id = this.selectedUserAndPlan.userWorkOutPlanId;

    const updatedPlan = {
      IsCompleted: this.editPlanForm.value.isCompleted,
      UpdatedAt: new Date().toISOString(),
    };

    this.workOutPlanService
      .updateWorkOutPlan(id, updatedPlan)
      .subscribe((response: any) => {
        this.toast.display('Plan updated successfully', 'success');
        this.showEditPlanModal = false;
        this.getAllPlanDetails();
      });
  }

  openViewModal(plan: any) {
    this.dataService.setUser(plan);
    this.router.navigateByUrl('plan-details/user-stats');
  }
}
