import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import { LucideAngularModule } from 'lucide-angular';
import { CartService } from '../services/cart-service';
import { TokenService } from '../services/token-service';
import { Toast } from '../toast/toast';

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [CommonModule, LucideAngularModule, Toast],
  templateUrl: './product-details.html',
  styleUrl: './product-details.css',
})
export class ProductDetails {
  productDetails: any = {};
  loggerdInUserId: number = 0;

  @ViewChild(Toast) toast!: Toast;

  constructor(
    private cartService: CartService,
    private tokenService: TokenService
  ) {}

  ngOnInit() {
    this.loggerdInUserId = parseInt(
      String(this.tokenService.getUserId() ?? '0'),
      10
    );
    const stored = sessionStorage.getItem('selectedProduct');
    this.productDetails = stored ? JSON.parse(stored) : {};
  }

  addToCart(product: any) {
    const userId = this.loggerdInUserId;

    this.cartService
      .getCartItemsByUserId(userId)
      .subscribe((cartItems: any[]) => {
        const existingItem = cartItems.find(
          (item) => item.productId === product.productId
        );
        if (existingItem) {
          const updatedItem = {
            userId: existingItem.userId,
            productId: existingItem.productId,
            quantity: existingItem.quantity + 1,
            price: existingItem.price,
            createdDate: existingItem.createdDate,
          };

          this.cartService
            .updateCartItem(existingItem.id, updatedItem)
            .subscribe(
              () => {
               this.toast.display("Product quantity updated","success");
              },
              (error) => console.error('Error updating cart item:', error)
            );
        } else {
          const newItem = {
            userId: this.loggerdInUserId,
            productId: product.productId,
            quantity: 1,
            price: product.price,
            createdDate: new Date().toISOString(),
          };

          this.cartService.addToCart([newItem]).subscribe(
            () => {
              this.toast.display("Product added to the cart","success")
            },
            (error) => console.error('Error adding product to cart:', error)
          );
        }
      });
  }
}
