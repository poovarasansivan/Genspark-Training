import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, PlusIcon, Minus, Trash } from 'lucide-angular';
import { CartService } from '../services/cart-service';
import { Router } from '@angular/router';
import { TokenService } from '../services/token-service';
import { Toast } from '../toast/toast';

@Component({
  selector: 'app-carts',
  imports: [FormsModule, CommonModule, LucideAngularModule, Toast],
  templateUrl: './carts.html',
  styleUrl: './carts.css',
})
export class Carts {
  @ViewChild(Toast) toast!: Toast;

  plusIcon = PlusIcon;
  minusIcon = Minus;
  trashIcon = Trash;

  loggedInUserId: number = 0;
  cartItems: any[] = [];
  fetchedCartItems: any[] = [];
  constructor(
    private cartService: CartService,
    private route: Router,
    private tokenService: TokenService
  ) {}

  ngOnInit() {
    this.loggedInUserId = parseInt(
      String(this.tokenService.getUserId() ?? '0'),
      10
    );
    this.loadCart();
  }

  loadCart() {
    this.cartService.getCartItemsByUserId(this.loggedInUserId).subscribe(
      (response) => {
        this.fetchedCartItems = response;
        this.cartItems = this.fetchedCartItems.map((item: any) => ({
          ...item,
          quantity: item.quantity || 1,
        }));
        this.toast.display('Cart Items Fetched Successfull', 'info');
        console.log('Fetched cart items:', this.cartItems);
      },
      (error) => {
        console.error('Error fetching cart items:', error);
      }
    );
  }

  increaseQuantity(item: any) {
    item.quantity++;
    this.cartService.updateCartItem(item.id, item).subscribe(
      () => {
        this.toast.display('Product Quantity Increased', 'success');
      },
      (error) => console.error('Error updating cart item:', error)
    );
  }

  decreaseQuantity(item: any) {
    if (item.quantity > 1) {
      item.quantity--;
      this.cartService.updateCartItem(item.id, item).subscribe(
        () => {
          this.toast.display('Product Quantity Decreased', 'success');
        },
        (error) => console.error('Error updating cart item:', error)
      );
    } else {
      this.removeItem(item);
    }
  }

  removeItem(item: any) {
    this.cartService.removeFromCart(item.id).subscribe(
      () => {
        this.cartItems = this.cartItems.filter((i) => i.id !== item.id);
        this.toast.display('Product Removed from Cart', 'success');
      },
      (error) => console.error('Error removing cart item:', error)
    );
  }

  getTotal(): number {
    return this.cartItems.reduce(
      (total, item) => total + item.price * item.quantity,
      0
    );
  }

  navigateToCheckout() {
    this.route.navigate(['/checkout-page']);
  }
}
