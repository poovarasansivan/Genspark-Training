import { Component, ViewChild } from '@angular/core';
import {
  LucideAngularModule,
  SearchIcon,
  CirclePlus,
  CircleX,
} from 'lucide-angular';
import { ManageCoachService } from '../service/manage-coach.service';
import { CommonModule } from '@angular/common';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { UserService } from '../service/user.service';
import { Popup } from '../popup/popup';
import { TokenService } from '../service/token.service';

@Component({
  selector: 'app-manage-coach',
  imports: [
    FormsModule,
    LucideAngularModule,
    CommonModule,
    ReactiveFormsModule,
    Popup,
  ],
  templateUrl: './manage-coach.html',
  styleUrl: './manage-coach.css',
})
export class ManageCoach {
  @ViewChild(Popup) toast!: Popup;

  searchIcon = SearchIcon;
  addIcon = CirclePlus;
  cancelIcon = CircleX;

  allCoaches: any[] = [];
  filteredCoaches: any[] = [];

  searchTerm = '';
  pageNumber = 1;
  pageSize = 10;
  totalPages = 0;
  pageNumbers: number[] = [];
  showAddCoachForm = false;
  showDeleteModal = false;
  currentUserRole: string | null = null;

  coaches: any[] = [];
  clients: any[] = [];
  selectedCoach: any = null;

  constructor(
    private coachservice: ManageCoachService,
    private userService: UserService,
    private tokenService: TokenService
  ) {}

  get currentUser() {
    return (this.currentUserRole = this.tokenService.getRole());
  }

  addMappingForm = new FormGroup({
    coach: new FormControl('', [Validators.required]),
    client: new FormControl('', [Validators.required]),
  });

  get coach() {
    return this.addMappingForm.get('coach');
  }

  get client() {
    return this.addMappingForm.get('client');
  }

  ngOnInit() {
    this.loadAllCoachMapping();
    this.loadAllUsers();
  }

  loadAllUsers() {
    this.userService.getAllUsers().subscribe({
      next: (res: any) => {
        const users = res?.data?.$values || [];
        this.coaches = users.filter((user: any) => user.role === 'Coach');
        this.clients = users.filter((user: any) => user.role === 'User');
      },
      error: (err) => {
        console.error('Error fetching users:', err);
      },
    });
  }

  loadAllCoachMapping() {
    console.log(this.currentUserRole);
    if (this.currentUser !== 'Admin') {
      this.coachservice
        .getCoachOnlyMapping(this.tokenService.getUserId()!)
        .subscribe({
          next: (response) => {
            this.allCoaches = response;
            this.applyFilters();
          },
          error: (error) => {
            console.error('Error loading coaches:', error);
          },
        });
      return;
    } else {
      this.coachservice.getAllCoaches().subscribe({
        next: (response) => {
          this.allCoaches = response;
          this.applyFilters();
        },
        error: (error) => {
          console.error('Error loading coaches:', error);
        },
      });
    }
  }

  applyFilters() {
    let filtered = [...this.allCoaches];
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(
        (coach) =>
          coach.coachName.toLowerCase().includes(term) ||
          coach.coachEmail.toLowerCase().includes(term) ||
          coach.clientName.toLowerCase().includes(term) ||
          coach.clientEmail.toLowerCase().includes(term)
      );
    }

    this.totalPages = Math.ceil(filtered.length / this.pageSize);
    this.pageNumbers = this.updatePageNumbers();

    const start = (this.pageNumber - 1) * this.pageSize;
    const end = start + this.pageSize;
    this.filteredCoaches = filtered.slice(start, end);
  }

  updatePageNumbers(): number[] {
    const pages: number[] = [];
    if (this.totalPages <= 7) {
      return Array.from({ length: this.totalPages }, (_, i) => i + 1);
    }

    pages.push(1);
    if (this.pageNumber > 4) pages.push(-1); // represents ...

    const startPage = Math.max(2, this.pageNumber - 1);
    const endPage = Math.min(this.totalPages - 1, this.pageNumber + 1);
    for (let i = startPage; i <= endPage; i++) pages.push(i);

    if (this.pageNumber < this.totalPages - 3) pages.push(-2);
    pages.push(this.totalPages);

    return pages;
  }

  changePage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.pageNumber = page;
    this.applyFilters();
  }

  onSearchInput(term: string) {
    this.searchTerm = term;
    this.pageNumber = 1;
    this.applyFilters();
  }

  addMapping() {
    const coachId = this.addMappingForm.value.coach;
    const clientId = this.addMappingForm.value.client;
    if (!coachId || !clientId) {
      this.toast.display('Please select both coach and client.', 'error');
      return;
    }
    this.coachservice.addCoachMapping(coachId, clientId).subscribe({
      next: (response) => {
        this.loadAllCoachMapping();
        this.addMappingForm.reset();
        this.showAddCoachForm = false;
        this.toast.display(
          'Coach-Client mapping added successfully.',
          'success'
        );
      },
      error: (error) => {
        console.error('Error adding mapping:', error);
        this.toast.display('Failed to add mapping. Please try again.', 'error');
      },
    });
  }

  openDeleteModal(mapping: any) {
    this.selectedCoach = mapping;
    console.log('Selected mapping for deletion:', this.selectedCoach);
    this.showDeleteModal = true;
  }

  deleteMapping() {
    if (!this.selectedCoach) {
      this.toast.display('No mapping selected for deletion.', 'error');
      return;
    }
    const coachId = this.selectedCoach.coachId;
    const clientId = this.selectedCoach.clientId;
    this.coachservice.deleteCoachMapping(coachId, clientId).subscribe({
      next: (response) => {
        this.loadAllCoachMapping();
        this.showDeleteModal = false;
        this.selectedCoach = null;
        this.toast.display(
          'Coach-Client mapping deleted successfully.',
          'success'
        );
      },
      error: (error) => {
        this.toast.display(
          'Failed to delete mapping. Please try again.',
          'error'
        );
      },
    });
  }
}
