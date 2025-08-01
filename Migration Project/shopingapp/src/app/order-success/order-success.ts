import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-order-success',
  imports: [],
  templateUrl: './order-success.html',
  styleUrl: './order-success.css',
})
export class OrderSuccess {
  constructor(private route: Router) {}

  goToProducts() {
    this.route.navigate(['/products']);
  }
}
