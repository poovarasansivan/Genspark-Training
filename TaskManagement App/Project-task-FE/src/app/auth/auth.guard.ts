import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from './auth.service';
import { CanActivateFn } from '@angular/router';


export const authGuard = (allowedRoles: string[] = []): CanActivateFn => {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);
    const toastr = inject(ToastrService);

    console.log('AuthGuard:', {
  user: auth.getUser(),
  isAuthenticated: auth.isAuthenticated(),
  allowedRoles
});

    // Not logged in (no token)
    if (!auth.isAuthenticated()) {
      toastr.warning('Please log in to access this page.', 'Authentication Required');
      router.navigateByUrl('/login');
      return false;
    }

    const user = auth.getUser();

    // Session exists but user info is missing
    if (!user) {
      toastr.error('Could not read session. Please refresh the page.', 'Temporary Issue');
      return false; //  No logout
    }

    // Logged in, but role not allowed
    if (!allowedRoles.includes(user.role)) {
      toastr.error("Access Denied: You don't have permission to access this page.", 'Unauthorized');
      router.navigateByUrl('/access-denied');
      return false;
    }

    return true;
  };
};


