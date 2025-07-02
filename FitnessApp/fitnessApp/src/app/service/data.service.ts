import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class DataService {
  private selectedUser = new BehaviorSubject<any | null>(null);
  selectedUser$ = this.selectedUser.asObservable();

  constructor() {
    const savedUser = localStorage.getItem('selectedUser');
    if (savedUser) {
      this.selectedUser.next(JSON.parse(savedUser));
    }
  }
  setUser(user: any) {
    localStorage.setItem('selectedUser', JSON.stringify(user));
    this.selectedUser.next(user);
  }

  clearUser() {
    localStorage.removeItem('selectedUser');
    localStorage.removeItem('userId');
    localStorage.removeItem('workOutPlanId');
    this.selectedUser.next(null);
  }
  
}
