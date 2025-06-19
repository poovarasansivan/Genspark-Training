import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import {
  debounce,
  debounceTime,
  distinct,
  distinctUntilChanged,
  fromEvent,
  map,
  Observable,
  startWith,
  switchMap,
} from 'rxjs';
import { User } from '../models/user';
import { UserService } from '../service/user.service';
import { CommonModule, NgFor } from '@angular/common';
import { Notification } from '../notification/notification';
import { FormsModule } from '@angular/forms';
import { Route, Router } from '@angular/router';

@Component({
  selector: 'app-userlist',
  standalone: true,
  imports: [CommonModule, Notification, FormsModule],
  templateUrl: './userlist.html',
  styleUrls: ['./userlist.css'],
})
export class Userlist implements OnInit {
  successMessage = '';
  errorMessage = '';
  showSuccess = false;
  showError = false;
  roleFilter = '';

  @ViewChild('searchInput', { static: true }) searchInput!: ElementRef;
  filteredUsers$!: Observable<User[]>;
  users$!: Observable<User[]>;

  constructor(private userService: UserService, private route: Router) {}

  ngOnInit(): void {
    this.filteredUsers$ = fromEvent(
      this.searchInput.nativeElement,
      'input'
    ).pipe(
      map((event: any) => event.target.value.trim().toLowerCase()),
      debounceTime(300),
      distinctUntilChanged(),
      startWith(''),
      switchMap((term: string) => this.userService.searchUser(term))
    );
    if (this.filteredUsers$ != null) {
     console.log('All users fetched')
    } else {
      this.errorMessage = 'Failed to fetch users details';
      this.showError = true;
      this.showSuccess = false;
    }
  }

  addUsers() {
    this.route.navigateByUrl('/add-user');
  }
}
