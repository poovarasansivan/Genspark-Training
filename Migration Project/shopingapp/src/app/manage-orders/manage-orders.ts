import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { Toast } from '../toast/toast';
import { TokenService } from '../services/token-service';
import { OrderService } from '../services/order-service';

@Component({
  selector: 'app-manage-orders',
  imports: [
    ReactiveFormsModule,
    FormsModule,
    CommonModule,
    LucideAngularModule,
    Toast,
  ],
  templateUrl: './manage-orders.html',
  styleUrl: './manage-orders.css',
})
export class ManageOrders {
  @ViewChild(Toast) toast!: Toast;
  orders: any[] = [];
  filteredOrders: any[] = [];
  OrderSearchTerm: string = '';

  showDeleteModal: boolean = false;
  showEditModal: boolean = false;

  orderFormsGroup = new FormGroup({
    status: new FormControl('', [Validators.required]),
  });

  get status() {
    return this.orderFormsGroup.get('status') as FormControl;
  }

  filters = {
    pageNumber: 1,
    pageSize: 5,
    userName: '',
    status: '',
    sortBy: '',
    sortDirection: '',
    searchTerm: '',
  };

  constructor(
    private tokenService: TokenService,
    private orderService: OrderService
  ) {}

  ngOnInit() {
    this.getOrders();
  }

  getOrders() {
    this.orderService.getPageniatedOrderDetails(this.filters).subscribe({
      next: (response: any) => {
        this.orders = response.orders || [];
        this.filteredOrders = [...this.orders];
        console.log('Fetched orders:', this.orders);
      },
      error: (error: any) => {
        console.error('Error fetching orders:', error);
        this.toast.display('Failed to fetch orders', 'error');
      },
    });
  }

  onSearchChange(searchTerm: string) {
    const search = searchTerm.trim().toLowerCase();

    this.filteredOrders = this.orders.filter(
      (order) =>
        order.id.toString().includes(search) ||
        order.userName?.toLowerCase().includes(search) ||
        order.productName?.toLowerCase().includes(search) ||
        order.status?.toLowerCase().includes(search) ||
        order.paymentMethod?.toLowerCase().includes(search) ||
        order.phoneNumber?.includes(search) ||
        order.address?.toLowerCase().includes(search)
    );
  }

  editOrder(order: any) {
    this.showEditModal = true;
  }

  openDeleteModal(order: any) {
    this.showDeleteModal = true;
  }

  closeEditModal() {
    this.showEditModal = false;
  }
  closeDeleteModel() {
    this.showDeleteModal = false;
  }

  confirmDeleteOrder() {}

  updateOrders() {}
}
