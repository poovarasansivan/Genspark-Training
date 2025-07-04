import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { NgZone } from '@angular/core';

import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

@Component({
  selector: 'app-payments',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, CommonModule],
  templateUrl: './payments.html',
  styleUrls: ['./payments.css'],
})
export class Payments {
  loading = false;
  paymentResult: { success: boolean; message: string } | null = null;

  paymentForm = new FormGroup({
    name: new FormControl('', [Validators.required]),
    email: new FormControl('', [Validators.required, Validators.email]),
    phone: new FormControl('', [Validators.required, Validators.pattern('^[0-9]{10}$')]),
    upiId: new FormControl('', [Validators.required]),
    amount: new FormControl('', [Validators.required, Validators.min(1)]),
  });

  get name() {
    return this.paymentForm.get('name');
  }

  get email() {
    return this.paymentForm.get('email');
  }

  get phone() {
    return this.paymentForm.get('phone');
  }

  get upiId() {
    return this.paymentForm.get('upiId');
  }

  get amount() {
    return this.paymentForm.get('amount');
  }
  constructor(private zone: NgZone) {}

  onSubmit() {
    if (this.paymentForm.invalid) {
      return;
    }
    this.loading = true;
    this.paymentResult = null;
    const formData = this.paymentForm.value;
    const options = {
      key: 'rzp_test_Fnr8qymkfdrY4q',
      amount: Number(formData.amount) * 100,
      currency: 'INR',
      name: 'Simulation Payments',
      email: formData.email,
      description: 'Test Transaction',
      handler: (response: any) => {
        this.zone.run(() => {
          this.loading = false;
          this.paymentResult = {
            success: true,
            message: `Payment successful! Payment ID: ${response.razorpay_payment_id}`,
          };
          setTimeout(()=>{
            this.paymentResult = null;
            this.loading = false;
            this.paymentForm.reset();
          }, 2000);
        });
      },
      modal: {
        ondismiss: () => {
          this.zone.run(() => {
            this.loading = false;
            this.paymentResult = {
              success: false,
              message: ' Payment cancelled by user.',
            };
            setTimeout(() => {
              this.paymentResult = null;
              this.loading = false;
              this.paymentForm.reset();
            }, 2000);
          });
        },
      },

      prefill: {
        method: 'upi',
        upi: {
          vpa: formData.upiId,
        },
        name: formData.name,
        email: formData.email,
        contact: formData.phone,
      },
      theme: {
        color: '#3f51b5',
      },
    };

    const rzp = new (window as any).Razorpay(options);
    rzp.open();
  }
}
