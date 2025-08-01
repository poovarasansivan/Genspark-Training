import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ModelService } from '../services/model-service';
import { CategoryService } from '../services/category-service';
import { ColorService } from '../services/color-service';
import { ProductService } from '../services/product-service';
import { LucideAngularModule, ArrowLeft, ArrowRight } from 'lucide-angular';
import { Toast } from '../toast/toast';
import { CreateNews } from "../create-news/create-news";

@Component({
  selector: 'app-manage-products',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    LucideAngularModule,
    Toast,
    CreateNews
],
  templateUrl: './manage-products.html',
  styleUrl: './manage-products.css',
})
export class ManageProducts {
  @ViewChild(Toast) toast!: Toast;

  arrowLeft = ArrowLeft;
  arrowRight = ArrowRight;
  categoryForm!: FormGroup;
  editProductForm!: FormGroup;
  modelForm!: FormGroup;
  colorForm!: FormGroup;
  productForm!: FormGroup;
  categories: any[] = [];
  filteredCategories: any[] = [];
  models: any[] = [];
  filteredModels: any[] = [];
  colors: any[] = [];
  filteredColors: any[] = [];
  ProductsFetched: any[] = [];
  filteredProducts: any[] = [];
  ProductSearchTerm = '';
  filters = {
    searchTerm: '',
    pageNumber: 1,
    pageSize: 5,
  };

  showModal = false;
  showModelModal = false;
  selectedProductId: any = null;
  showcolorModal = false;
  showEditModal = false;
  showCreateProductModal = false;
  showDeleteModal = false;
  searchTerm = '';
  searchModelTerm = '';
  searchColorTerm = '';

  categoryPage = 1;
  modelPage = 1;
  colorPage = 1;
  itemsPerPage = 7;
  selectedProductForEdit: any = null;

  constructor(
    private fb: FormBuilder,
    private modelService: ModelService,
    private categoryService: CategoryService,
    private colorService: ColorService,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.getModels();
    this.getCategories();
    this.getColors();
    this.getProducts();
    this.categoryForm = this.fb.group({
      name: ['', Validators.required],
    });
    this.modelForm = this.fb.group({
      name: ['', Validators.required],
    });
    this.colorForm = this.fb.group({
      name: ['', Validators.required],
    });
    this.productForm = this.fb.group({
      productName: ['', Validators.required, Validators.minLength(3)],
      productImage: ['', Validators.required],
      productDescription: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(0)]],
      category: ['', Validators.required],
      model: ['', Validators.required],
      color: ['', Validators.required],
      sellStartDate: ['', Validators.required],
      sellEndDate: ['', Validators.required],
      isNew: ['', Validators.required],
    });
    this.editProductForm = this.fb.group({
      productName: ['', Validators.required],
      productImage: ['', Validators.required],
      productDescription: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(0)]],
      category: ['', Validators.required],
      model: ['', Validators.required],
      color: ['', Validators.required],
      sellStartDate: ['', Validators.required],
      sellEndDate: ['', Validators.required],
      isNew: ['', Validators.required],
    });
  }

  getModels() {
    this.modelService.getModels().subscribe((data) => {
      this.models = data;
      this.filteredModels = [...this.models];
    });
  }

  getCategories() {
    this.categoryService.getCategories().subscribe((data) => {
      this.categories = data;
      this.filteredCategories = [...this.categories];
    });
  }

  getColors() {
    this.colorService.getColors().subscribe((data) => {
      this.colors = data;
      this.filteredColors = [...this.colors];
    });
  }

  getProducts() {
    this.productService.getProducts(this.filters).subscribe((data) => {
      this.ProductsFetched = data.products;
      this.filteredProducts = [...this.ProductsFetched];
    });
  }

  openModal() {
    this.categoryForm.reset();
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  openModelCreateModal() {
    this.modelForm.reset();
    this.showModelModal = true;
  }

  closeModelCreateModal() {
    this.showModelModal = false;
  }

  openColorCreateModal() {
    this.colorForm.reset();
    this.showcolorModal = true;
  }

  closeColorCreateModal() {
    this.showcolorModal = false;
  }

  openDeleteModal(product: any) {
    this.selectedProductId = product.productId;
    this.showDeleteModal = true;
  }

  closeDeleteModal() {
    this.showDeleteModal = false;
  }

  get paginatedCategories() {
    const start = (this.categoryPage - 1) * this.itemsPerPage;
    return this.filteredCategories.slice(start, start + this.itemsPerPage);
  }

  get paginatedModels() {
    const start = (this.modelPage - 1) * this.itemsPerPage;
    return this.filteredModels.slice(start, start + this.itemsPerPage);
  }

  get paginatedColors() {
    const start = (this.colorPage - 1) * this.itemsPerPage;
    return this.filteredColors.slice(start, start + this.itemsPerPage);
  }

  get totalCategoryPages() {
    return Math.ceil(this.filteredCategories.length / this.itemsPerPage);
  }

  get totalModelPages() {
    return Math.ceil(this.filteredModels.length / this.itemsPerPage);
  }

  get totalColorPages() {
    return Math.ceil(this.filteredColors.length / this.itemsPerPage);
  }

  changeCategoryPage(delta: number) {
    const newPage = this.categoryPage + delta;
    if (newPage > 0 && newPage <= this.totalCategoryPages)
      this.categoryPage = newPage;
  }

  changeModelPage(delta: number) {
    const newPage = this.modelPage + delta;
    if (newPage > 0 && newPage <= this.totalModelPages)
      this.modelPage = newPage;
  }

  changeColorPage(delta: number) {
    const newPage = this.colorPage + delta;
    if (newPage > 0 && newPage <= this.totalColorPages)
      this.colorPage = newPage;
  }

  saveProduct() {
    if (this.productForm.invalid) {
      this.closeCreateProductModal();
      this.productForm.reset();
      this.toast.display('Product Data is Invalid', 'error');
      return;
    }

    const sellSDate = new Date(
      this.productForm.value.sellStartDate + 'T00:00:00'
    );
    const sellEDate = new Date(
      this.productForm.value.sellEndDate + 'T00:00:00'
    );

    const formData = new FormData();
    formData.append('ProductName', this.productForm.value.productName);
    formData.append('Image', this.productForm.value.productImage); // Should be File object
    formData.append('Description', this.productForm.value.productDescription);
    formData.append('Price', this.productForm.value.price.toString());
    formData.append('CategoryId', this.productForm.value.category.toString());
    formData.append('ColorId', this.productForm.value.color.toString());
    formData.append('ModelId', this.productForm.value.model.toString());
    formData.append('SellStartDate', sellSDate.toISOString());
    formData.append('SellEndDate', sellEDate.toISOString());
    formData.append('IsNew', this.productForm.value.isNew.toString());

    this.productService.createProduct(formData).subscribe(
      (data) => {
        this.productForm.reset();
        this.closeCreateProductModal();
        this.toast.display('Product added successfully', 'success');
        this.getProducts();
      },
      (error) => {
        console.error('Error creating product:', error);
        this.toast.display('Error in adding the Product', 'error');
      }
    );
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.productForm.patchValue({
        productImage: file,
      });
      this.productForm.get('productImage')?.updateValueAndValidity();
    }
  }

  onImageSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.editProductForm.patchValue({ productImage: file });
      this.editProductForm.get('productImage')?.updateValueAndValidity();
    }
  }

  updateProduct() {
    const sellSDate = this.editProductForm.value.sellStartDate;
    const sellEDate = this.editProductForm.value.sellEndDate;

    const formData = new FormData();
    formData.append('ProductId', this.selectedProductForEdit.productId);
    formData.append(
      'ProductName',
      this.editProductForm.value.productName || ''
    );
    formData.append('Image', this.editProductForm.value.productImage);
    formData.append(
      'Description',
      this.editProductForm.value.productDescription || ''
    );
    formData.append(
      'Price',
      this.editProductForm.value.price.toString() || '0'
    );
    formData.append(
      'CategoryId',
      this.editProductForm.value.category.toString() || '0'
    );
    formData.append(
      'ColorId',
      this.editProductForm.value.color.toString() || '0'
    );
    formData.append(
      'ModelId',
      this.editProductForm.value.model.toString() || '0'
    );
    formData.append(
      'SellStartDate',
      sellSDate
        ? new Date(sellSDate + 'T00:00:00').toISOString()
        : this.selectedProductForEdit.sellStartDate
    );
    formData.append(
      'SellEndDate',
      sellEDate
        ? new Date(sellEDate + 'T00:00:00').toISOString()
        : this.selectedProductForEdit.sellEndDate
    );
    formData.append(
      'IsNew',
      this.editProductForm.value.isNew.toString() || 'false'
    );
    this.productService.updateProduct(formData).subscribe(
      (data) => {
        this.editProductForm.reset();
        this.closeEditModal();
        this.toast.display('Product updated successfully', 'success');
        this.getProducts();
      },
      (error) => {
        console.error('Error updating product:', error);
        this.toast.display('Failed to update product', 'error');
      }
    );
  }

  confirmDeleteProduct() {
    this.showDeleteModal = false;
    this.productService.deleteProduct(this.selectedProductId).subscribe(
      (data) => {
        this.toast.display("Product deleted successfully","success");
        console.log('Product deleted successfully:', data);
      },
      (error) => {
        console.error('Error deleting product:', error);
        this.toast.display("Failed to delete product","error");
      }
    );
    this.selectedProductId = null;
    this.getProducts();
  }

  saveCategory() {
    if (this.categoryForm.invalid) {
      this.closeModal();
      this.categoryForm.reset();
      this.toast.display("Enter a valid category name","error");
      return;
    }
    this.categoryService.addCategory(this.categoryForm.value.name).subscribe(
      (data) => {
        this.categoryForm.reset();
        this.closeModal();
        this.toast.display("Category added successfully","success");
      },
      (error) => {
        console.error('Error adding category:', error);
        this.toast.display("Failed to add category","error");
      }
    );
    this.getCategories();
  }

  saveModel() {
    if (this.modelForm.invalid) {
      this.closeModelCreateModal();
      this.modelForm.reset();
      this.toast.display("Enter a valid model name","error");
      return;
    }
    this.modelService.addModle(this.modelForm.value.name).subscribe(
      (data) => {
        this.modelForm.reset();
        this.closeModelCreateModal();
        this.toast.display("Model created successfully","success");
      },
      (error) => {
        console.error('Error adding model:', error);
        this.toast.display("Failed to add Model","error");
      }
    );
    this.getModels();
  }

  saveColor() {
    if (this.colorForm.invalid) {
      this.closeColorCreateModal();
      this.colorForm.reset();
      this.toast.display("Please enter a valid color name","error");
      return;
    }
    this.colorService.addColor(this.colorForm.value.name).subscribe(
      (data) => {
        this.colorForm.reset();
        this.closeColorCreateModal();
        this.toast.display("New color created successfully","success");
      },
      (error) => {
        console.error('Error adding color:', error);
        this.toast.display("Failed to add color. Please try again.","error");
      }
    );
    this.getColors();
  }

  searchCategory(term: string) {
    this.filteredCategories = this.categories.filter(
      (c) => c.name && c.name.toLowerCase().includes(term.toLowerCase())
    );
  }

  searchModel(term: string) {
    this.filteredModels = this.models.filter(
      (m) =>
        m.modelName && m.modelName.toLowerCase().includes(term.toLowerCase())
    );
  }

  openCreateProductModal(): void {
    this.showCreateProductModal = true;
  }

  closeCreateProductModal(): void {
    this.showCreateProductModal = false;
  }

  editProduct(product: any): void {
    this.showEditModal = true;
    this.selectedProductForEdit = product;
  }

  closeEditModal(): void {
    this.showEditModal = false;
  }

  onSearchChange(ProductSearchTerm: string): void {
    const searchTerm = ProductSearchTerm.trim().toLowerCase();

    this.filteredProducts = this.ProductsFetched.filter((product) => {
      return (
        product.productName?.toLowerCase().includes(searchTerm) ||
        product.category?.name?.toLowerCase().includes(searchTerm) ||
        product.model?.modelName?.toLowerCase().includes(searchTerm) ||
        product.color?.colorName?.toLowerCase().includes(searchTerm)
      );
    });
  }

  searchColor(term: string) {
    this.filteredColors = this.colors.filter(
      (c) =>
        c.colorName && c.colorName.toLowerCase().includes(term.toLowerCase())
    );
  }

  deleteCategory(categoryId: number) {
    this.categoryService.deleteCategory(categoryId).subscribe((data) => {
      this.toast.display("Category deleted successfully","success");
    });
    this.getCategories();
  }

  deleteModel(modelId: number) {
    this.modelService.deleteModel(modelId).subscribe((data) => {
      this.toast.display("Model deleted successfully","success");
    });
    this.getModels();
  }

  deleteColor(colorId: number) {
    this.colorService.deleteColor(colorId).subscribe((data) => {
      this.toast.display("Color deleted successfully","success");
    });
    this.getColors();
  }

  changePage(delta: number) {
    this.filters.pageNumber += delta;
    if (this.filters.pageNumber < 1) this.filters.pageNumber = 1;
    this.getProducts();
  }
}
