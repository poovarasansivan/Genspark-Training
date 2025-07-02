import { Routes } from '@angular/router';
import { Login } from './login/login';
import { MainLayout } from './layouts/main-layout/main-layout';
import { Home } from './home/home';
import { AuthLayout } from './layouts/auth-layout/auth-layout';
import { Forgotpassword } from './forgotpassword/forgotpassword';
import { AuthGuard } from './auth-guard';
import { Verifyemail } from './verifyemail/verifyemail';
import { CoachHome } from './coach-home/coach-home';
import { ClientHome } from './client-home/client-home';
import { ManageUser } from './manage-user/manage-user';
import { ManageCoach } from './manage-coach/manage-coach';
import { Plans } from './plans/plans';
import { PlanDetails } from './plan-details/plan-details';
import { WorkoutLogs } from './workout-logs/workout-logs';
import { WeeklyUpdate } from './weekly-update/weekly-update';
import { Profile } from './profile/profile';
import { UserStats } from './user-stats/user-stats';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    canActivate: [AuthGuard],
    children: [
      { path: 'home', component: Home },
      { path: 'coach-home', component: CoachHome },
      { path: 'client-home', component: ClientHome },
      { path: 'manage-user', component: ManageUser },
      { path: 'manage-coach', component: ManageCoach },
      { path: 'manage-plan', component: Plans },
      { path: 'manage-plan/plan-details/:id', component: PlanDetails },
      { path: 'workout-logs', component: WorkoutLogs },
      { path: 'weekly-progress', component: WeeklyUpdate },
      { path: 'profile', component: Profile },
      { path: 'plan-details/user-stats', component: UserStats },
    ],
  },
  {
    path: '',
    component: AuthLayout,
    children: [
      { path: 'login', component: Login },
      { path: 'verify-email', component: Verifyemail },
      { path: 'reset-password', component: Forgotpassword },
    ],
  },
  { path: '**', redirectTo: 'login' },
];
