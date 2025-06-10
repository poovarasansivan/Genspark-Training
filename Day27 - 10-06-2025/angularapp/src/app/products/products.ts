import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class Products {
  count = 0;

  addToCart(product: any) {
    product.quantity += 1;
    this.count += 1;
  }

  removeFromCart(product: any) {
    if (product.quantity > 0) {
      product.quantity -= 1;
      this.count -= 1;
    }
  }

  products: any[] = [
    {
      id: 1,
      name: 'Laptop',
      price: 1000,
      description: 'High performance laptop',
      image: 'assets/p1.png',
      quantity: 0,
    },
    {
      id: 2,
      name: 'Smartphone',
      price: 500,
      description: 'Latest model smartphone',
      image: 'assets/p2.png',
      quantity: 0,
    },
    {
      id: 3,
      name: 'Tablet',
      price: 300,
      description: 'Portable tablet with great features',
      image: 'assets/p3.png',
      quantity: 0,
    },
    {
      id: 4,
      name: 'Smartwatch',
      price: 200,
      description: 'Stylish smartwatch with health tracking',
      image: 'assets/p4.png',
      quantity: 0,
    },
  ];
}
