import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NotificationService } from './service/notification.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected title = 'fitnessApp';

  constructor(private notificationService: NotificationService) {}
  ngOnInit() {
    this.notificationService.startConnection();
  }
}
