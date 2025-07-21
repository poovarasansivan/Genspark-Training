import { Routes } from '@angular/router';
import { List } from './list/list';
import { Details } from './details/details';
import { Form } from './form/form';

export const routes: Routes = [
  { path: '', component: List },
  { path: 'details/:id', component: Details},
  { path: 'create-course', component: Form},
];
