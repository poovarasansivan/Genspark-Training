import { Routes } from '@angular/router';
import { Payments } from './payments/payments';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: Payments },
];
