import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-notification',
  imports: [CommonModule, FormsModule],
  templateUrl: './notification.html',
  styleUrl: './notification.css',
})
export class Notification {
  @Input() title = '';
  @Input() message = '';
  @Input() type: 'success' | 'error' = 'success';

  visible = true;

  ngOnInit(): void {
    setTimeout(() => (this.visible = false), 3000);
  }

  close() {
    this.visible = false;
  }
}
