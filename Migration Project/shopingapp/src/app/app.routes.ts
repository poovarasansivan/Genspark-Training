import { Routes } from '@angular/router';
import { LoginComponent } from './login/login';
import { Register } from './register/register';
import { Home } from './home/home';
import { MainLayout } from './layouts/main-layout/main-layout';
import { AuthGuard } from './auth-guard';
import { AuthLayout } from './layouts/auth-layout/auth-layout';
import { Products } from './products/products';
import { ProductDetails } from './product-details/product-details';
import { CreateProduct } from './create-product/create-product';
import { ManageProducts } from './manage-products/manage-products';
import { Carts } from './carts/carts';
import { Orders } from './orders/orders';
import { CheckoutPage } from './checkout-page/checkout-page';
import { OrderSuccess } from './order-success/order-success';
import { Contact } from './contact/contact';
import { About } from './about/about';
import { News } from './news/news';
import { NewsDetails } from './news-details/news-details';
import { ManageOrders } from './manage-orders/manage-orders';
import { ManageTickets } from './manage-tickets/manage-tickets';
import { CreateNews } from './create-news/create-news';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: '/home', pathMatch: 'full' },
      { path: 'home', component: Home },
      { path: 'products', component: Products },
      { path: 'products-details/:id', component: ProductDetails },
      { path: 'create-product', component: CreateProduct },
      { path: 'manage-shop', component: ManageProducts },
      { path: 'carts', component: Carts },
      { path: 'orders', component: Orders },
      { path: 'checkout-page', component: CheckoutPage },
      { path: 'order-completed', component: OrderSuccess },
      { path: 'contact', component: Contact },
      { path: 'about', component: About },
      { path: 'blog', component: News },
      { path: 'blog-details/:id', component: NewsDetails },
      { path: 'manage-orders', component: ManageOrders },
      { path: 'manage-tickets', component: ManageTickets },
      { path: 'create-blog', component: CreateNews },
    ],
  },
  {
    path: '',
    component: AuthLayout,
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'register', component: Register },
    ],
  },
  { path: '**', redirectTo: '/login' },
];
