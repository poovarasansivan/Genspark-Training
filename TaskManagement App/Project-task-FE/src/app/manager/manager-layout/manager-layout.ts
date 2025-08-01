import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { ManagerNavbarComponent } from '../manager-navbar/manager-navbar';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-manager-layout',
  standalone: true,
  imports: [RouterOutlet, ManagerNavbarComponent,CommonModule],
  templateUrl: './manager-layout.html',
  styleUrl: './manager-layout.css'
})
export class ManagerLayoutComponent {
  constructor(private router: Router) {}

  get shouldShowSidebar(): boolean {
    const hiddenRoutes = ['/access-denied', '/not-found', '/404'];
    return !hiddenRoutes.includes(this.router.url);
  }
}
