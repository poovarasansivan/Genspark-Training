import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './toast.html',
  styleUrl: './toast.css',
})
export class Toast {
  show = false;
  message = '';
  type: 'success' | 'error' | 'info' = 'info';

  constructor(private cdRef: ChangeDetectorRef) {}

  display(message: string, type: 'success' | 'error' | 'info' = 'info') {
    this.message = message;
    this.type = type;
    this.show = true;
    this.cdRef.detectChanges(); 
    setTimeout(() => {
      this.show = false;
      this.cdRef.detectChanges(); 
    }, 3000);
  }
}
