import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../auth/auth.service';
import { RouterModule } from '@angular/router';
import { NotificationService } from '../../Notifications/notification.service';

@Component({
  selector: 'app-manager-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink,RouterModule],
  templateUrl: './manager-navbar.html',
  styleUrl: './manager-navbar.css'
})
export class ManagerNavbarComponent {
  user = JSON.parse(sessionStorage.getItem('user') || '{}');
  unreadCount = 0;

  constructor(private auth: AuthService, private router: Router,private notificationService: NotificationService) {}

  ngOnInit() {
  this.notificationService.notifications$.subscribe(notifs => {
    this.unreadCount = notifs.filter(n => !n.isRead).length;
  });
}

  isSidebarOpen = false;

  toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  logout() {
    this.auth.logout();
  }

  isManager() { return this.user.role === 'Manager'; }
  isTeamMember() { return this.user.role === 'TeamMember'; }
}
