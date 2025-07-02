import { Component, OnInit } from '@angular/core';
import { LucideAngularModule, UserIcon, Workflow, Eye } from 'lucide-angular';
import { TokenService } from '../service/token.service';
import { UserService } from '../service/user.service';
import PlanSummary from '../model/CoachHome';
import { UserPlanService } from '../service/user-plan.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-coach-home',
  standalone: true,
  imports: [LucideAngularModule, CommonModule],
  templateUrl: './coach-home.html',
  styleUrl: './coach-home.css',
})
export class CoachHome implements OnInit {
  userIcon = UserIcon;
  planIcon = Workflow;
  viewIcon = Eye;

  motivationalQuote = '';
  coachName: string = '';
  plans: PlanSummary[] = [];
  activeUsersCount = 0;
  completedUsersCount = 0;
  uniquePlansCount = 0;

  constructor(
    private tokenService: TokenService,
    private userService: UserService,
    private userPlanService: UserPlanService
  ) {}

  ngOnInit(): void {
    const coachId = this.tokenService.getUserId();
    this.getUsername();

    Promise.all([
      this.userPlanService.getWorkOutPlans().toPromise(),
      this.userService.getPlanAnalysis().toPromise(),
    ]).then(([plansRes, userPlansRes]) => {
      const allPlans = (plansRes as any)?.data?.$values || [];
      const userPlans = userPlansRes?.data?.$values || [];

      const coachPlans = userPlans.filter((p: any) => p.coachId === coachId);

      const groupedPlans = coachPlans.reduce(
        (acc: Record<string, any[]>, entry: any) => {
          if (!acc[entry.workOutPlanId]) acc[entry.workOutPlanId] = [];
          acc[entry.workOutPlanId].push(entry);
          return acc;
        },
        {}
      );

      let active = 0;
      let completed = 0;

      this.plans = Object.entries(groupedPlans).map(([planId, items]) => {
        const itemsArray = items as any[];
        const usersCount = itemsArray.length;
        const completedUsers = itemsArray.filter(
          (p) => p.isCompleted === 'Completed'
        ).length;

        active += usersCount - completedUsers;
        completed += completedUsers;

        const planMeta = allPlans.find((p: any) => p.id === planId);

        return {
          planId,
          planName: itemsArray[0].workOutPlanName,
          usersCount,
          completedUsers,
          startDate: planMeta?.startDate || 'N/A',
          endDate: planMeta?.endDate || 'N/A',
        };
      });

      this.activeUsersCount = active;
      this.completedUsersCount = completed;
      this.uniquePlansCount = this.plans.length;
    });
  }

  getUsername(): string | null {
    const username = this.tokenService.getUsername();
    if (username) {
      this.coachName = username;
      return username;
    } else {
      console.warn('Username not found in token');
      return null;
    }
  }
}
