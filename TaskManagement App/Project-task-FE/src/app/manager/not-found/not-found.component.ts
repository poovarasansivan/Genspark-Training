import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="not-found">
      <h1>404 - Page Not Found</h1>
      <p>The page you're looking for doesn't exist.</p>
      <p>Return to the <a routerLink="/dashboard">dashboard</a>.</p>
    </div>
  `,
  styles: [`
    .not-found {
      text-align: center;
      padding: 80px 20px;
      color: #555;
    }

    .not-found h1 {
      font-size: 48px;
      color: #d32f2f;
      margin-bottom: 20px;
    }

    .not-found p {
      font-size: 18px;
    }
  `]
})
export class NotFoundComponent {}
