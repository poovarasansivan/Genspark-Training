import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { Toast } from '../toast/toast';

@Component({
  selector: 'app-create-product',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, Toast],
  templateUrl: './create-product.html',
  styleUrl: './create-product.css',
})
export class CreateProduct {
  @ViewChild(Toast) toast!: Toast;

  step = 1;
  productForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.productForm = this.fb.group({
      category: ['', Validators.required],
      model: ['', Validators.required],
      color: ['', Validators.required],
      productName: ['', Validators.required],
      description: ['', Validators.required],
      image: ['', Validators.required],
      price: [null, [Validators.required, Validators.min(1)]],
    });
  }

  nextStep() {
    if (
      this.productForm.get('category')?.valid &&
      this.productForm.get('model')?.valid &&
      this.productForm.get('color')?.valid
    ) {
      this.step = 2;
    }
  }

  prevStep() {
    this.step = 1;
  }

  submitProduct() {}
}
