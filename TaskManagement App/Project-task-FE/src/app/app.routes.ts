import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login';
import { ManagerDashboard } from './manager/manager-dashboard/manager-dashboard';
import { authGuard } from './auth/auth.guard';
import { ManagerLayoutComponent } from './manager/manager-layout/manager-layout';
import { CreateUserComponent } from './manager/Create-User/create-user.component';
import { UserListComponent } from './manager/user-list/user-list';
import { CreateTaskComponent } from './manager/create-task/create-task';
import { TaskListComponent } from './manager/task-list/task-list.component';
import { AccessDeniedComponent } from './manager/AccessDeniedComponent/access-denied.component';
import { NotificationsComponent } from './Notifications/notifications.component';
import { NotFoundComponent } from './manager/not-found/not-found.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: '',
    component:ManagerLayoutComponent,
    canActivate: [authGuard(['Manager','TeamMember'])],
    children: [

    { path: 'dashboard', component: ManagerDashboard },
    { path: 'users/create',component:CreateUserComponent,canActivate: [authGuard(['Manager'])]},
    { path: 'users',component:UserListComponent, canActivate: [authGuard(['Manager'])] },
    { path: 'task/create',component:CreateTaskComponent, canActivate: [authGuard(['Manager'])]},
    { path: 'tasks',component:TaskListComponent,},
    { path: 'notifications', component: NotificationsComponent },
    { path: 'access-denied', component: AccessDeniedComponent },

  ]
    
  },
  {
    path: '**',
    component: NotFoundComponent
  }
];