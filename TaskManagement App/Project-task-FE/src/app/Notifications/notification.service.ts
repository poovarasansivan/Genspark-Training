import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Notification } from './notification.model';
import { AuthService } from '../auth/auth.service'; 

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private notifications: Notification[] = [];
  private notificationsSubject = new BehaviorSubject<Notification[]>([]);
  public notifications$ = this.notificationsSubject.asObservable();

  private storageKey: string;

  constructor(private authService: AuthService) {
    const userId = this.authService.getUserId();
    this.storageKey = `notifications-${userId}`;
    this.loadFromStorage();
  }

  private loadFromStorage() {
    const stored = localStorage.getItem(this.storageKey);
    this.notifications = stored ? JSON.parse(stored) : [];
    this.notificationsSubject.next(this.notifications);
  }

  private saveToStorage() {
    localStorage.setItem(this.storageKey, JSON.stringify(this.notifications));
  }

  addNotification(notification: Notification) {
    this.notifications.unshift(notification);
    this.saveToStorage();
    this.notificationsSubject.next(this.notifications);
  }

  markAllAsRead() {
    this.notifications.forEach(n => n.isRead = true);
    this.saveToStorage();
    this.notificationsSubject.next(this.notifications);
  }

  getUnreadCount(): number {
    return this.notifications.filter(n => !n.isRead).length;
  }
}
