import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet, Router } from '@angular/router';
import { AuthService } from './auth/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected title = 'Project-task-FE';

  private authService = inject(AuthService);
  private router = inject(Router);

  ngOnInit(): void
  {
    {
    const token = this.authService.getAccessToken();
    const refreshToken = this.authService.getRefreshToken();
    const user = this.authService.getUser();

    if (!token && refreshToken) {
      this.authService.refreshToken().subscribe(success => {
        if (!success) {
        this.router.navigate(['/login']);
      }
      });
    } else if (!token || !user) {
    this.router.navigate(['/login']);
  }
  }
  }
}
