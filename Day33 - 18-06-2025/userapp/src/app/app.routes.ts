import { Routes } from '@angular/router';
import { Usercard } from './usercard/usercard';
import { Form } from './form/form';

export const routes: Routes = [
    {path: '', redirectTo: 'usercard', pathMatch: 'full'},
    {path: 'usercard', component: Usercard},
    {path: 'adduser', component: Form}
];
