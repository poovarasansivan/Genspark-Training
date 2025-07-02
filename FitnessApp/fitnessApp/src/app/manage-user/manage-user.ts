import { CommonModule } from '@angular/common';
import { Component, NgModule, ViewChild } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import {
  LucideAngularModule,
  SearchIcon,
  CirclePlus,
  CircleX,
} from 'lucide-angular';
import { Users } from '../model/Users';
import { UserService } from '../service/user.service';
import { Popup } from '../popup/popup';
import { UserFilter } from '../model/UserFilter';

export function passwordMatchValidator(
  form: AbstractControl
): ValidationErrors | null {
  const password = form.get('password')?.value;
  const confirmPassword = form.get('confirmPassword')?.value;
  return password === confirmPassword ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-manage-user',
  imports: [
    CommonModule,
    FormsModule,
    LucideAngularModule,
    Popup,
    ReactiveFormsModule,
  ],
  templateUrl: './manage-user.html',
  styleUrl: './manage-user.css',
})
export class ManageUser {
  @ViewChild(Popup) toast!: Popup;


  searchIcon = SearchIcon;
  addIcon = CirclePlus;
  cancelIcon = CircleX;

  // User data
  users: Users[] = [];
  allUsers: Users[] = [];
  totalUsers = 0;

  // Filters
  searchTerm: string = '';
  roleFilter: string = '';
  statusFilter: string = '';

  isFilterApplied = false;
  pageNumbers: number[] = [];

  filter: UserFilter = {
    pageNumber: 1,
    pageSize: 10,
    isActive: undefined,
    sortBy: 'name',
    sortDirection: 'asc',
  };

  showAddUserModal: boolean = false;
  showEditUserModal: boolean = false;
  selectedUser: Users | null = null;
  showDeleteUserModal: boolean = false;

  addUserForm = new FormGroup(
    {
      name: new FormControl('', [Validators.required]),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(6),
      ]),
      confirmPassword: new FormControl('', [
        Validators.required,
        Validators.minLength(6),
      ]),
      role: new FormControl('', [Validators.required]),
      status: new FormControl('', [Validators.required]),
    },
    { validators: passwordMatchValidator }
  );

  editUserForm = new FormGroup({
    ename: new FormControl('', [Validators.required]),
    eemail: new FormControl('', [Validators.required, Validators.email]),
    erole: new FormControl('', [Validators.required]),
    estatus: new FormControl('', [Validators.required]),
  });

  get ename() {
    return this.editUserForm.get('ename')!;
  }

  get eemail() {
    return this.editUserForm.get('eemail')!;
  }

  get erole() {
    return this.editUserForm.get('erole')!;
  }

  get estatus() {
    return this.editUserForm.get('estatus')!;
  }

  get name() {
    return this.addUserForm.get('name')!;
  }

  get email() {
    return this.addUserForm.get('email')!;
  }

  get password() {
    return this.addUserForm.get('password')!;
  }

  get confirmPassword() {
    return this.addUserForm.get('confirmPassword')!;
  }

  get role() {
    return this.addUserForm.get('role')!;
  }

  get status() {
    return this.addUserForm.get('status')!;
  }

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.loadAllUsers();
  }

  toggleModal(id: string) {
    const modal = document.getElementById(id);
    if (modal) modal.classList.toggle('hidden');
  }

  loadAllUsers() {
    const bigPageSizeFilter: UserFilter = {
      ...this.filter,
      pageNumber: 1,
      pageSize: 1000, 
    };

    this.userService.getUsersPagination(bigPageSizeFilter).subscribe((res) => {
      this.allUsers = res.data;
      this.totalUsers = this.allUsers.length;

      const pageSize = this.filter.pageSize ?? 10;
      const start = (this.filter.pageNumber! - 1) * pageSize;
      const end = start + pageSize;

      this.users = this.allUsers.slice(start, end);
      this.pageNumbers = this.updatePageNumbers();
    });
  }

  handleSearchInput() {
    if (!this.isFilterApplied) this.filterUsersClientSide();
  }

  filterUsersClientSide() {
    let filtered = [...this.allUsers];

    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(
        (user) =>
          user.name.toLowerCase().includes(term) ||
          user.email.toLowerCase().includes(term)
      );
    }

    if (this.roleFilter) {
      filtered = filtered.filter((user) => user.role === this.roleFilter);
    }

    if (this.statusFilter !== '') {
      filtered = filtered.filter(
        (user) => user.isActive === (this.statusFilter === 'true')
      );
    }

    this.users = filtered;
    this.totalUsers = filtered.length;
  }

  applyFilters() {
    this.isFilterApplied = true;
    this.filter.pageNumber = 1;

    this.filter.isActive =
      this.statusFilter === ''
        ? undefined
        : this.statusFilter === 'true'
        ? true
        : false;

    this.userService.getUsersPagination(this.filter).subscribe((res) => {
      this.users = res.data;
      this.totalUsers = res.totalCount;
    });
    this.pageNumbers = this.updatePageNumbers();
  }

  onRoleChange(role: string) {
    this.roleFilter = role;
    this.filterUsersClientSide();
  }
  
  onStatusChange(status: string) {
    this.statusFilter = status;
    this.filterUsersClientSide();
  }

  resetFilters() {
    this.searchTerm = '';
    this.roleFilter = '';
    this.statusFilter = '';
    this.isFilterApplied = false;

    this.filter = {
      pageNumber: 1,
      pageSize: 10,
      isActive: undefined,
      sortBy: 'name',
      sortDirection: 'asc',
    };

    this.users = [...this.allUsers];
    this.totalUsers = this.users.length;
    this.pageNumbers = this.updatePageNumbers();
  }

  updatePageNumbers(): number[] {
    const pageSize = this.filter.pageSize ?? 10;
    const totalPages = Math.ceil(this.totalUsers / pageSize);
    const currentPage = this.filter.pageNumber ?? 1;

    const pages: number[] = [];

    if (totalPages <= 7) {
      return Array.from({ length: totalPages }, (_, i) => i + 1);
    }

    pages.push(1);

    if (currentPage > 4) {
      pages.push(-1);
    }

    const startPage = Math.max(2, currentPage - 1);
    const endPage = Math.min(totalPages - 1, currentPage + 1);

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    if (currentPage < totalPages - 3) {
      pages.push(-2);
    }

    pages.push(totalPages);

    return pages;
  }

  changePage(page: number) {
    this.filter.pageNumber = page;
    if (this.isFilterApplied) {
      this.applyFilters();
    } else {
      const pageSize = this.filter.pageSize ?? 10;
      const start = (page - 1) * pageSize;
      const end = start + pageSize;
      this.users = this.allUsers.slice(start, end);
    }
  }

  addUser() {
    if (this.addUserForm.invalid) {
      this.addUserForm.markAllAsTouched();
      this.toast.display(
        'Please fill in all required fields correctly.',
        'error'
      );
      return;
    }

    const newUser = {
      id: '',
      name: this.addUserForm.value.name || '',
      email: this.addUserForm.value.email || '',
      password: this.addUserForm.value.password || '',
      role: this.addUserForm.value.role || '',
      isActive: this.addUserForm.value.status === 'true' ? true : false,
    };
    this.userService.addNewUser(newUser).subscribe({
      next: (response) => {
        this.toast.display('User added successfully', 'success');
        this.loadAllUsers();
        this.showAddUserModal = false;
        this.addUserForm.reset();
      },
      error: (error) => {
        console.error('Error adding user:', error.message);
        this.toast.display('Failed to add user', 'error');
      },
    });
  }

  openAddUserModal() {
    this.showAddUserModal = true;
    this.addUserForm.reset();
    this.toggleModal('addUserModal');
  }

  openEditModal(user: Users) {
    this.selectedUser = user;
    this.showEditUserModal = true;
  }

  openDeleteModal(user: Users) {
    this.selectedUser = user;
    this.showDeleteUserModal = true;
  }

  closeModals() {
    this.showEditUserModal = false;
    this.showDeleteUserModal = false;
    this.selectedUser = null;
  }

  deleteUser() {
    if (!this.selectedUser) return;

    this.userService.deleteUser(this.selectedUser.id).subscribe({
      next: () => {
        this.toast.display('User deleted successfully.', 'success');
        this.loadAllUsers();
        this.showDeleteUserModal = false;
      },
      error: (err) => {
        console.error('Error deleting user:', err);
        this.toast.display('Failed to delete user.', 'error');
      },
    });
  }

  editUser() {
    if (!this.selectedUser) return;

    const updatedUser: Users = {
      ...this.selectedUser,
      name: this.editUserForm.value.ename || '',
      email: this.editUserForm.value.eemail || '',
      role: this.editUserForm.value.erole || '',
      isActive: this.editUserForm.value.estatus === 'true' ? true : false,
    };

    this.userService.updateUser(this.selectedUser.id, updatedUser).subscribe({
      next: (response) => {
        this.toast.display('User updated successfully', 'success');
        this.loadAllUsers();
        this.closeModals();
      },
      error: (error) => {
        console.error('Error updating user:', error);
        this.toast.display('Failed to update user', 'error');
      },
    });
  }
}
