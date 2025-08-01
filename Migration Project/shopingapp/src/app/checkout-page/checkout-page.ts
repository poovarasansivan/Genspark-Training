import { Component, ViewChild } from '@angular/core';
import { CartService } from '../services/cart-service';
import { Router } from '@angular/router';
import { TokenService } from '../services/token-service';
import { CommonModule } from '@angular/common';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { OrderService } from '../services/order-service';
import { Toast } from '../toast/toast';


@Component({
  selector: 'app-checkout-page',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, Toast],
  templateUrl: './checkout-page.html',
  styleUrl: './checkout-page.css',
})
export class CheckoutPage {
  @ViewChild(Toast) toast!: Toast;

  showPayPalButton = true;
  loggedInUserId: number = 0;
  loggedInUserName: string = '';
  cartItems: any = [];

  constructor(
    private route: Router,
    private cartService: CartService,
    private tokenService: TokenService,
    private http: HttpClient,
    private orderService: OrderService
  ) {}

  checkoutForm = new FormGroup({
    mobile: new FormControl('', [
      Validators.required,
      Validators.pattern('^[0-9]{10}$'),
    ]),
    address: new FormControl('', [Validators.required]),
    landmark: new FormControl('', [Validators.required]),
    city: new FormControl('', [Validators.required]),
    state: new FormControl('', [Validators.required]),
    country: new FormControl('', [Validators.required]),
    postalcode: new FormControl('', [
      Validators.required,
      Validators.pattern('^[0-9]{6}$'),
    ]),
  });

  get name() {
    return this.checkoutForm.get('name');
  }

  get mobile() {
    return this.checkoutForm.get('mobile');
  }

  get address() {
    return this.checkoutForm.get('address');
  }

  get landmark() {
    return this.checkoutForm.get('landmark');
  }

  get city() {
    return this.checkoutForm.get('city');
  }

  get state() {
    return this.checkoutForm.get('state');
  }

  get country() {
    return this.checkoutForm.get('country');
  }

  get postalcode() {
    return this.checkoutForm.get('postalcode');
  }

  ngOnInit() {
    this.loggedInUserId = parseInt(
      String(this.tokenService.getUserId() ?? '0'),
      10
    );
    this.loggedInUserName = String(this.tokenService.getUserName() ?? '');
    this.loadCart();
  }

  loadCart() {
    this.cartService.getCartItemsByUserId(this.loggedInUserId).subscribe(
      (response) => {
        this.cartItems = response;
        console.log('Fetched cart items:', this.cartItems);
      },
      (error) => {
        console.error('Error fetching cart items:', error);
      }
    );
  }

  get subtotal() {
    return this.cartItems.reduce(
      (total: number, item: any) => total + item.price * item.quantity,
      0
    );
  }

  get shipping() {
    return 10;
  }

  get tax() {
    return this.subtotal * 0.05;
  }

  get total() {
    return this.subtotal + this.shipping + this.tax;
  }

  async placeOrder() {
    if (this.checkoutForm.invalid) {
      this.checkoutForm.markAllAsTouched();
      return;
    }
    const fullAddressString = `${this.checkoutForm.value.address}, ${this.checkoutForm.value.landmark}, ${this.checkoutForm.value.city}, ${this.checkoutForm.value.state}, ${this.checkoutForm.value.country}, ${this.checkoutForm.value.postalcode}`;

    const orderDetails = {
      userId: this.loggedInUserId,
      status: 'Pending',
      address: fullAddressString,
      phoneNumber: this.checkoutForm.value.mobile,
      paymentStatus: 'Prepaid',
      paymentMethod: 'Online',
      orderDate: new Date(),
      products: this.cartItems.map((item: any) => ({
        productId: item.productId,
        quantity: item.quantity,
        totalAmount: item.price * item.quantity,
      })),
    };

    try {
      const orderResponse = await this.orderService
        .placeOrder(orderDetails)
        .toPromise();
      console.log('Order placed successfully:', orderResponse);
      this.toast.display('Order Placed Successfully', 'success');
      const productIdsToRemove = this.cartItems.map((item: any) => item.id);

      const cartClearResponse = await this.cartService
        .removeFromCart(productIdsToRemove)
        .toPromise();

      console.log('Cart cleared successfully:', cartClearResponse);

      this.renderPayPalButtons(this.total);
    } catch (error) {
      console.error('Error placing order:', error);
      this.toast.display('Failed to place order', 'error');
    }
  }
  renderPayPalButtons(amount: number) {
    this.showPayPalButton = true;
    setTimeout(() => {
      (window as any).paypal
        .Buttons({
          createOrder: (data: any, actions: any) => {
            return actions.order.create({
              purchase_units: [
                {
                  amount: {
                    value: amount.toFixed(2),
                  },
                },
              ],
            });
          },
          onApprove: async (data: any, actions: any) => {
            const paymentDetails = await actions.order.capture();
            console.log('Payment approved:', paymentDetails);
            this.toast.display('Payment Success via PayPal', 'success');

            this.route.navigate(['/orders']);
          },
          onError: (err: any) => {
            console.error('PayPal Payment Error:', err);
            this.toast.display('PayPal Payment Failed', 'error');
          },
        })
        .render('#paypal-button-container');
    }, 100); // small delay to ensure container is ready
  }
}
