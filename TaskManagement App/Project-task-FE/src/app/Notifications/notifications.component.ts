import { Component, OnInit } from '@angular/core';
import { Notification } from './notification.model';
import { NotificationService } from './notification.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notifications',
  imports: [CommonModule],
  standalone: true,
  styleUrl: './notifications.component.css',
  templateUrl: './notifications.component.html',
})

export class NotificationsComponent implements OnInit {
  notifications: Notification[] = [];

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.notificationService.notifications$.subscribe(n => {
      this.notifications = n;
    });
  }

  markAllAsRead() {
    this.notificationService.markAllAsRead();
  }
}