import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { LucideAngularModule, Eye, View } from 'lucide-angular';
import { UserPlanService } from '../service/user-plan.service';
import { AllWorkOutPlan } from '../model/AllWorkOutPlan';
import { UserService } from '../service/user.service';
import { TokenService } from '../service/token.service';
import { QuoteService } from '../service/quote.service';
import { Router } from '@angular/router';
import { NotificationService } from '../service/notification.service';
import { Popup } from '../popup/popup';

@Component({
  selector: 'app-client-home',
  standalone: true,
  imports: [LucideAngularModule, CommonModule, Popup],
  templateUrl: './client-home.html',
  styleUrl: './client-home.css',
})
export class ClientHome implements OnInit {
  @ViewChild(Popup) popup!: Popup;

  viewIcon = Eye;

  motivationalQuote = '';
  clientName: string = '';
  WorkOutPlans: AllWorkOutPlan[] = [];
  activeplan: AllWorkOutPlan[] = [];
  completedplan: AllWorkOutPlan[] = [];

  constructor(
    private tokenService: TokenService,
    private userPlanService: UserPlanService,
    private route: Router,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    const userId = this.tokenService.getUserId();
    this.getUsername();
    this.userPlanService.getAllWorkOutPlans().subscribe({
      next: (response: any) => {
        const allPlans: AllWorkOutPlan[] = response.data?.$values || [];

        const userPlans = allPlans.filter((plan) => plan.userId === userId);

        this.completedplan = userPlans.filter(
          (plan) => plan.isCompleted === 'Completed'
        );
        this.activeplan = userPlans.filter(
          (plan) => plan.isCompleted !== 'Completed'
        ); // e.g., "On Going"

        this.WorkOutPlans = userPlans;
      },
      error: (err) => {
        console.error('Error fetching workout plans:', err);
      },
    });
    this.notificationService.startConnection();
  }

  ngAfterViewInit() {
    this.notificationService.notifications$.subscribe((message) => {
      if (message) {
        this.popup.display(message, 'info');
      }
    });
  }

  getUsername(): string | null {
    const username = this.tokenService.getUsername();
    if (username) {
      this.clientName = username;
      return username;
    } else {
      console.warn('Username not found in token');
      return null;
    }
  }
  viewLogs() {
    this.route.navigate(['/manage-plan']);
  }
}
