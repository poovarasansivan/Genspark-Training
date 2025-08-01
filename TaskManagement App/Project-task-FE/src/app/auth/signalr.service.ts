import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AuthService } from '../auth/auth.service';
import { NotificationService } from './../Notifications/notification.service';
import { v4 as uuidv4 } from 'uuid';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection!: signalR.HubConnection;

  constructor(private auth: AuthService, private notificationService: NotificationService) {}

  startConnection(): void {
     const token = this.auth.getAccessToken();
    if (!token) {
        console.warn('No access token available yet, skipping SignalR connection.');
        return;
    }
    this.hubConnection = new signalR.HubConnectionBuilder()
  .withUrl(`http://localhost:5093/taskHub`, {
    accessTokenFactory: () => this.auth.getAccessToken() || ''
  })
  .withAutomaticReconnect()
  .configureLogging(signalR.LogLevel.Information)
  .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connected'))
      .catch(err => console.error('SignalR Error:', err));

    this.registerListeners();
  }

  
private registerListeners(): void {
  const add = (type: 'TaskCreated' | 'TaskUpdated' | 'TaskDeleted' | 'TaskAssigned' | 'TaskUnassigned' | 'TaskReopened', task: any) => {
  const actionMap: Record<typeof type, string> = {
    TaskCreated: 'created',
    TaskUpdated: 'updated',
    TaskDeleted: 'deleted',
    TaskAssigned: 'assigned',
    TaskUnassigned: 'unassigned',
    TaskReopened: 'reopened'
  };

  const notification = {
    id: uuidv4(),
    message: `${task.title} - ${actionMap[type]}`,
    title: task.title,
    type,
    createdAt: new Date().toISOString(),
    isRead: false,
    Status: task.status,
    PreviousStatus: task.previousStatus,
    UpdatedByName: task.updatedByName,
    AssignedToName: task.assignedToName,
    CreatedByName: task.createdByName,
    UnassignedByName: task.unassignedByName, 
    ...task
  };

  this.notificationService.addNotification(notification);
};

  this.hubConnection.on('TaskCreated', task => add('TaskCreated', task));
  this.hubConnection.on('TaskUpdated', task => add('TaskUpdated', task));
  this.hubConnection.on('TaskDeleted', task => add('TaskDeleted', task));
  this.hubConnection.on('TaskAssigned', task => add('TaskAssigned', task)); 
  this.hubConnection.on('TaskUnassigned', task => {
  console.log('[TaskUnassigned]', task); 
  add('TaskUnassigned', task);
});

}
}
