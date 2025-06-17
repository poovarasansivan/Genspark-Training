import { Component, inject, HostListener } from '@angular/core';
import { LucideAngularModule, Search, ShoppingCartIcon, ViewIcon } from 'lucide-angular';
import { FormsModule } from '@angular/forms';
import {
  Subject,
  debounceTime,
  distinctUntilChanged,
  switchMap,
  tap,
} from 'rxjs';
import { ProductModel } from '../model/productmodel';
import { ProductService } from '../service/product.service';
import { CommonModule } from '@angular/common';
import { HighlightPipe } from '../highlight-pipe';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, HighlightPipe],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  searchIcon = Search;
  cartIcon = ShoppingCartIcon;
  viewIcon = ViewIcon;

  searchTerm: string = '';
  searchSubject = new Subject<string>();

  loading: boolean = false;
  products: ProductModel[] = [];
  hasMore = true;

  private skip = 0;
  private limit = 10;
  private term = '';

  private productService = inject(ProductService);

  handleSearch() {
    this.skip = 0;
    this.products = [];
    this.hasMore = true;
    this.searchSubject.next(this.searchTerm);
  }
  constructor(public route: Router) {
  }

  ngOnInit(): void {
    this.searchSubject
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        tap(() => {
          this.loading = true;
          this.skip = 0;
        }),
        switchMap((query) => {
          this.term = query || '';
          return this.productService.getSearchProducts(this.term, this.skip);
        })
      )
      .subscribe({
        next: (data: any) => {
          this.products = data.products as ProductModel[];
          this.loading = false;
          this.hasMore = data.products.length === this.limit;
          console.log('Products loaded:', this.products);
        },
        error: (err) => {
          console.error('Error fetching products:', err);
          this.loading = false;
        },
      });
  }

  loadMoreProducts() {
    if (!this.hasMore || this.loading) return;

    this.loading = true;
    this.skip += this.limit;

    this.productService.getSearchProducts(this.term, this.skip).subscribe({
      next: (data) => {
        if (data.products.length > 0) {
          this.products.push(...data.products);
          this.hasMore = data.products.length === this.limit;
        } else {
          this.hasMore = false;
        }
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading more products:', err);
        this.loading = false;
      },
    });
  }

  viewProduct(productId: number) {
    window.location.href = `/product/${productId}`;
    this.route.navigate(['/product', productId]);
  }
  @HostListener('window:scroll', [])
  onScroll(): void {
    const threshold = 150;
    const position = window.innerHeight + window.scrollY;
    const height = document.body.offsetHeight;

    if (position > height - threshold) {
      this.loadMoreProducts();
    }
  }

  scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
