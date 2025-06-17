import { Routes } from '@angular/router';
import { Home } from './home/home';
import { About } from './about/about';
import {Login} from './login/login';
import { AuthGuard } from './auth-guard';
import { ProductDetails } from './product-details/product-details';

export const routes: Routes = [
    {path: '', redirectTo: 'login', pathMatch: 'full'},
    {path:'login' , component: Login, title: 'Login' },
    {path: 'home', component: Home, title: 'Home',canActivate:[AuthGuard] },
    {path: 'product/:id', component: ProductDetails, title: 'Product Details',canActivate:[AuthGuard] },
    {path: 'about', component: About, title: 'About',canActivate:[AuthGuard] },
];
