import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-customer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './customer.html',
  styleUrl: './customer.css',
})
export class Customer {
  customers: any[] = [
    { id: 1, name: 'John Doe', email: 'johndoe@gmail.com', likeCount: 0, liked: false },
    { id: 2, name: 'Jane Smith', email: 'janesmith@gmail.com', likeCount: 0, liked: false },
    { id: 3, name: 'Poovarasan', email: 'poovarasan@gmail.com', likeCount: 0, liked: false },
  ];

  like(id: number) {
    const customer = this.customers.find(c => c.id === id);
    if (customer) {
      customer.liked = !customer.liked;
      customer.likeCount += customer.liked ? 1 : -1;
    }
  }

  dislike(customer: any) {
    if (customer.likeCount > 0) {
      customer.likeCount -= 1;
      customer.liked = false; 
    }
  }
}
