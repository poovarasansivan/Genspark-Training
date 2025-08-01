import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../Create-User/user.service';
import { UserResponseDto } from '../Create-User/user.types';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { HttpErrorResponse } from '@angular/common/http';
import { EditUserModalComponent } from "../EditUserModalComponent/edit-user-modal.component";
import { Subject, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, FormsModule, EditUserModalComponent],
  templateUrl: './user-list.html',
  styleUrls: ['./user-list.css'],
})
export class UserListComponent implements OnInit, OnDestroy {
  users: UserResponseDto[] = [];
  loading = false;

  searchTerm: string = '';
  roleFilter = '';
  page = 1;
  pageSize = 5;
  totalPages = 0;

  showEditModal = false;
  selectedUser?: UserResponseDto;

  private searchSubject = new Subject<string>();
  private searchSubscription!: Subscription;

  constructor(private userService: UserService, private toastr: ToastrService) {}

  ngOnInit(): void {
    this.fetchUsers();

    this.searchSubscription = this.searchSubject.pipe(debounceTime(500)).subscribe(search => {
      this.searchTerm = search;
      this.page = 1;
      this.fetchUsers();
    });
  }

  ngOnDestroy(): void {
    this.searchSubscription?.unsubscribe();
  }

  onSearchInput(value: string) {
    this.searchSubject.next(value);
  }

  setRoleFilter(role: string) {
    this.roleFilter = role;
    this.page = 1;
    this.fetchUsers();
  }

  fetchUsers() {
    this.loading = true;

    this.userService.getAllUsers(this.page, this.pageSize, this.searchTerm, this.roleFilter).subscribe({
      next: res => {
        this.users = res.data;
        this.totalPages = res.pagination.totalPages;
        this.loading = false;
      },
      error: err => {
        this.toastr.error(err?.error?.message || 'Failed to load users');
        this.loading = false;
      }
    });
  }

  goToPage(page: number) {
    this.page = page;
    this.fetchUsers();
  }

  getPageRange(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  getInitials(name: string): string {
    const [first = '', last = ''] = name.split(' ');
    return first.charAt(0).toUpperCase() + last.charAt(0).toUpperCase();
  }

  openEditModal(user: UserResponseDto) {
    this.selectedUser = user;
    this.showEditModal = true;
    document.body.style.overflow = 'hidden';
  }

  onModalClose() {
    this.showEditModal = false;
    this.selectedUser = undefined;
    document.body.style.overflow = 'auto';
  }

  onUserUpdated() {
    this.fetchUsers();
  }

  deleteUser(id: string) {
    if (!confirm('Are you sure you want to delete this user?')) return;

    this.userService.deleteUser(id).subscribe({
      next: () => {
        this.toastr.success('User deleted');
        this.fetchUsers();
      },
      error: (err: HttpErrorResponse) => {
        this.toastr.error(err.error?.message || 'Delete failed');
      }
    });
  }
}
