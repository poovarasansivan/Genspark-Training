import { Routes } from '@angular/router';
import { Adduser } from './adduser/adduser';
import { Userlist } from './userlist/userlist'; 

export const routes: Routes = [
    {path: '', redirectTo: '/home', pathMatch: 'full'},
    {path: 'home', component: Userlist},
    {path: 'add-user', component: Adduser}
];
