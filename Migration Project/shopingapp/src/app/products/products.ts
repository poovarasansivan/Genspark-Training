import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, ViewChild } from '@angular/core';
import {
  LucideAngularModule,
  ArrowLeft,
  Search,
  PlusCircleIcon,
} from 'lucide-angular';
import { ProductService } from '../services/product-service';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TokenService } from '../services/token-service';
import { CartService } from '../services/cart-service';
import { Toast } from '../toast/toast';

@Component({
  selector: 'app-products',
  imports: [LucideAngularModule, CommonModule, FormsModule, Toast],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class Products {
  @ViewChild(Toast) toast!: Toast;

  BackIcon = ArrowLeft;
  SearchIcon = Search;
  PlusIcon = PlusCircleIcon;
  products: any[] = [];
  filteredProducts: any[] = [];
  searchTerm: string = '';
  totalPages = 0;
  totalItems = 0;
  pages: number[] = [];
  selectedCategory: string = '';
  filters = {
    search: '',
    minPrice: null,
    maxPrice: null,
    category: '',
    pageSize: 10,
    pageNumber: 1,
  };

  constructor(
    private productService: ProductService,
    private route: Router,
    private tokenService: TokenService,
    private cartService: CartService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.selectedCategory = sessionStorage.getItem('selectedCategory') || '';
    this.getProducts();
  }

  getProducts() {
    if (this.selectedCategory) {
      this.filters.category = this.selectedCategory;
    }
    this.productService.getProducts(this.filters).subscribe({
      next: (response) => {
        this.products = response.products;
        this.filteredProducts = [...this.products];
        this.cdr.detectChanges();
        this.totalItems = response.totalCount;
        this.totalPages = Math.ceil(this.totalItems / this.filters.pageSize);
        this.pages = Array.from({ length: this.totalPages }, (_, i) => i + 1);
        this.toast.display('Products Fetched SuccessFully', 'info');
      },
      error: (error) => {
        this.toast.display('Error fetching products', 'error');
        console.error('Error fetching products:', error);
      },
    });
  }
  onSearchProducts() {
    if (this.searchTerm.trim() === '') {
      this.filteredProducts = [...this.products];
    } else {
      this.filteredProducts = this.products.filter((product) =>
        product.productName
          .toLowerCase()
          .includes(this.searchTerm.toLowerCase())
      );
    }
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.filters.pageNumber = page;
    this.getProducts();
  }

  onFilterChange() {
    this.filters.pageNumber = 1;
    this.getProducts();
  }

  createProduct() {
    this.route.navigate(['/manage-products']);
  }

  addToCart(product: any) {
    const userId = parseInt(String(this.tokenService.getUserId() ?? '0'), 10);

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
            createdDate: existingItem.createdDate,
          };

          this.cartService
            .updateCartItem(existingItem.id, updatedItem)
            .subscribe(
              () => {
                this.toast.display(
                  'Product quantity updated in cart!',
                  'success'
                );
              },
              (error) => console.error('Error updating cart item:', error)
            );
        } else {
          const newItem = {
            userId,
            productId: product.productId,
            quantity: 1,
            price: product.price,
            createdDate: new Date().toISOString(),
          };

          this.cartService.addToCart([newItem]).subscribe(
            () => {
              this.toast.display('Product added to cart!', 'success');
            },
            (error) => console.error('Error adding product to cart:', error)
          );
        }
      });
  }

  viewProductDetails(product: any) {
    sessionStorage.setItem('selectedProduct', JSON.stringify(product));
    this.route.navigate(['/products-details', product.productId]);
  }

  ngOnDestroy() {
    sessionStorage.removeItem('selectedCategory');
  }
}
