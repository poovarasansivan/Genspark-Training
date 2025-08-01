import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-footer',
  imports: [CommonModule],
  templateUrl: './footer.html',
  styleUrl: './footer.css',
})
export class Footer {
  categories = [
    'Laptops & Computers',
    'Cameras & Photography',
    'Smart Phones & Tablets',
    'Video Games & Consoles',
    'Waterproof Headphones',
  ];

  customerCare = [
    'My Account',
    'Discount',
    'Returns',
    'Orders History',
    'Order Tracking',
  ];

  pages = [
    'Blog',
    'Browse the Shop',
    'Category',
    'Pre-Built Pages',
    'Visual Composer Elements',
    'WooCommerce Pages',
  ];
}
