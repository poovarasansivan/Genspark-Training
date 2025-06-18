import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserModel } from '../models/usermodel';
import { UserService } from '../service/user.service';
import { LucideAngularModule, View } from 'lucide-angular';
import { Route, Router } from '@angular/router';
@Component({
  selector: 'app-usercard',
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './usercard.html',
  styleUrl: './usercard.css',
})
export class Usercard {
  viewIcon = View;
  allUsers: UserModel[] = [];
  displayedUsers: UserModel[] = [];
  genderFilter: string = '';
  roleFilter: string = '';
  stateFilter: string = '';
  ageFilter: string = '';

  constructor(private userService: UserService, private route: Router) {}

  applyFilters(): void {
    this.displayedUsers = this.allUsers.filter(
      (user) =>
        (!this.genderFilter ||
          user.gender.toLowerCase() ===
            this.genderFilter.trim().toLowerCase()) &&
        (!this.roleFilter ||
          user.role.toLowerCase() === this.roleFilter.trim().toLowerCase()) &&
        (!this.stateFilter ||
          user.address?.state?.toLowerCase() ===
            this.stateFilter.trim().toLowerCase()) &&
        (!this.ageFilter || this.isAgeInRange(user.age, this.ageFilter))
    );
  }

  isAgeInRange(age: number, range: string): boolean {
    if (range === '66+') return age >= 66;
    const [min, max] = range.split('-').map(Number);
    return age >= min && age <= max;
  }

ngOnInit(): void {
  this.userService.getUsers().subscribe((users) => {
    this.allUsers = users;
    const newUserJson = localStorage.getItem('user');
    if (newUserJson) {
      try {
        const newUser: UserModel = JSON.parse(newUserJson);
        this.allUsers.push(newUser);
      } catch (e) {
        console.error('Error parsing newUser from localStorage', e);
      }
    }
    this.displayedUsers = [...this.allUsers];
  });
}


  addUsers(): void {
    this.route.navigateByUrl('/adduser');
  }

  viewDetails(userId: number): void {
    this.route.navigate(['/userdetails', userId]);
  }
}
