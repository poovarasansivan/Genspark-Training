import { Component } from '@angular/core';
import { LucideAngularModule, UserIcon, Workflow } from 'lucide-angular';
import { BarChart } from '../charts/bar-chart/bar-chart';
import { Users } from '../model/Users';
import { UserService } from '../service/user.service';
import { BarChart2 } from '../charts/bar-chart2/bar-chart2';
import { TokenService } from '../service/token.service';

@Component({
  selector: 'app-home',
  imports: [LucideAngularModule, BarChart, BarChart2],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  userIcon = UserIcon;
  planIcon = Workflow;
  AdminName: string = '';
  constructor(
    private userService: UserService,
    private tokenService: TokenService
  ) {}
  
  acitveUserCount: number = 0;
  inactiveUserCount: number = 0;
  coachUserCount: number = 0;
  planCount: number = 0;

  ngOnInit() {
    this.getUsername();

    this.userService.getAllUsers().subscribe({
      next: (response: any) => {
        const users: Users[] = response.data?.$values ?? [];
        const activeUsers = users.filter((user: any) => user.isActive);
        const inactiveUsers = users.filter((user: any) => !user.isActive);
        const coachUsers = users.filter((user: any) => user.role === 'Coach');
        this.acitveUserCount = activeUsers.length;
        this.inactiveUserCount = inactiveUsers.length;
        this.coachUserCount = coachUsers.length;
      },
    });

    this.userService.getPlanCount().subscribe({
      next: (response: any) => {
        this.planCount = response.data.$values.length;
      },
      error: (error) => {
        console.error('Error fetching plan count:', error);
      },
    });
  }

  getUsername(): string | null {
    const username = this.tokenService.getUsername();
    if (username) {
      this.AdminName = username;
      return username;
    } else {
      console.warn('Username not found in token');
      return null;
    }
  }
}
