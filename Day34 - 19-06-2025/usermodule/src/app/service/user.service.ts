import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { User } from '../models/user';

@Injectable({ providedIn: 'root' })
export class UserService {
  private initialUsers: User[] = [
    { username: 'Poovarasan', email: 'pooovarasan@gmail.com', password: 'Poo@12345', role: 'Admin' },
    { username: 'Praveenraja', email: 'praveenraja@gmail.com', password: 'Praveen@12345', role: 'User' },
    { username: 'Prasad', email: 'prasad@gmail.com', password: 'Prasad@12345', role: 'User' },
  ];

  private users$ = new BehaviorSubject<User[]>(this.initialUsers);

  getUsers(): Observable<User[]> {
    return this.users$.asObservable();
  }

  searchUser(term: string): Observable<User[]> {
    const filtered = this.users$.value.filter(
      (user) =>
        user.username.toLowerCase().includes(term) ||
        user.role.toLowerCase().includes(term)
    );
    return of(filtered);
  }
  
  addUser(user: User) {
    const current = this.users$.value;
    this.users$.next([...current, user]);
  }
}
