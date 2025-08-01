import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [CommonModule],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  categories = [
    { name: 'Mobiles', image: 'assets/categories/mobile.png' },
    { name: 'Laptops', image: 'assets/categories/laptop.png' },
    { name: 'Furnitures', image: 'assets/categories/chair.png' },
    { name: 'Monitors', image: 'assets/categories/monitors.png' },
    { name: 'Shoes', image: 'assets/categories/shoe.png' },
    { name: 'Toys', image: 'assets/categories/toys.png' },
  ];

  offers = [
    {
      image: 'assets/offers/quality.png',
      title: 'Quality Products',
      description: 'We ensure the best quality for our products.',
    },
    {
      image: 'assets/offers/fast.png',
      title: 'Fast Delivery',
      description: 'Get your products delivered quickly and safely.',
    },
    {
      image: 'assets/offers/support.jpg',
      title: '24/7 Support',
      description: 'We’re here to help, anytime you need us.',
    },
    {
      image: 'assets/offers/return.png',
      title: 'Easy Returns',
      description: 'Hassle-free returns for your convenience.',
    },
  ];

  latestNews = [
    {
      image: 'assets/sample.png',
      title: 'New Arrivals',
      description: 'Check out the latest products in our store.',
    },
    {
      image: 'assets/sample.png',
      title: 'Seasonal Sale',
      description: 'Don’t miss our seasonal discounts and offers.',
    },
    {
      image: 'assets/sample.png',
      title: 'Customer Reviews',
      description: 'See what our customers are saying about us.',
    },
    {
      image: 'assets/sample.png',
      title: 'Customer Reviews',
      description: 'See what our customers are saying about us.',
    },
  ];

  constructor(private router: Router) {}

  goToCategory(category: string) {
    sessionStorage.setItem('selectedCategory', category);
    this.router.navigate(['/products']);
  }
}
