import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-access-denied',
  standalone: true,
  imports: [CommonModule,RouterModule],
  template: `
    <div class="access-denied">
      <h1>🚫 Access Denied</h1>
      <p>You do not have permission to access this page.</p>
      <p>Return to the <a routerLink="/dashboard">dashboard</a>.</p>
    </div>
  `,
  styles: [`
    .access-denied {
      text-align: center;
      padding: 80px 20px;
      color: #555;
    }

    .access-denied h1 {
      font-size: 48px;
      color: #d32f2f;
      margin-bottom: 20px;
    }

    .access-denied p {
      font-size: 18px;
    }
    .access-denied {
      position: fixed;
      top: 0;
      left: 0;
      width: 100vw;
      height: 100vh;
      background: #fff;
      z-index: 9999;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      color: #d32f2f;
    }
  `]
})
export class AccessDeniedComponent {}
