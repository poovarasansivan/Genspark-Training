import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import { OrderService } from '../services/order-service';
import { TokenService } from '../services/token-service';
import { About } from '../about/about';
import { Toast } from '../toast/toast';

@Component({
  selector: 'app-orders',
  imports: [CommonModule, Toast],
  templateUrl: './orders.html',
  styleUrl: './orders.css',
})
export class Orders {
  products: any[] = [];
  loggedInUserId: any = null;
  constructor(
    private orderService: OrderService,
    private tokenService: TokenService
  ) {}

  ngOnInit() {
    this.loggedInUserId = parseInt(
      String(this.tokenService.getUserId() ?? '0'),
      10
    );
    this.loadOrders();
  }

  loadOrders() {
    this.orderService
      .getOrdersByUserId(this.loggedInUserId)
      .subscribe((data: any) => {
        this.products = data;
      });
  }
}
