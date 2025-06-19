import { CommonModule, NgFor, NgIf } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-notification',
  imports: [CommonModule, NgIf],
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
