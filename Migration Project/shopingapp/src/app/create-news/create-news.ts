import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { LucideAngularModule, ArrowRight, ArrowLeft } from 'lucide-angular';
import { NewsService } from '../services/news-service';
import { TokenService } from '../services/token-service';
import { Toast } from '../toast/toast';

@Component({
  selector: 'app-create-news',
  imports: [
    CommonModule,
    FormsModule,
    LucideAngularModule,
    ReactiveFormsModule,
    Toast,
  ],
  templateUrl: './create-news.html',
  styleUrl: './create-news.css',
})
export class CreateNews {
  @ViewChild(Toast) toast!: Toast;

  NewsSearchTerm: string = '';
  arrowRight = ArrowRight;
  arrowLeft = ArrowLeft;
  showNewsModal: boolean = false;
  loggedInUserId: number = 0;
  filteredNews: any[] = [];
  filters = {
    searchTerm: '',
    pageNumber: 1,
    pageSize: 5,
  };

  editNewsItem: any = null;
  showEditModal: boolean = false;
  showDeleteModal: boolean = false;
  newsToDelete: any = null;

  newsFormGroup = new FormGroup({
    title: new FormControl('', [Validators.required]),
    shortDescription: new FormControl('', [Validators.required]),
    content: new FormControl('', [Validators.required]),
    image: new FormControl('', [Validators.required]),
    status: new FormControl('', [Validators.required]),
    createdDate: new FormControl('', [Validators.required]),
  });

  get title() {
    return this.newsFormGroup.get('title') as FormControl;
  }

  get shortDescription() {
    return this.newsFormGroup.get('shortDescription') as FormControl;
  }

  get content() {
    return this.newsFormGroup.get('content') as FormControl;
  }

  get image() {
    return this.newsFormGroup.get('image') as FormControl;
  }
  get status() {
    return this.newsFormGroup.get('status') as FormControl;
  }
  get createdDate() {
    return this.newsFormGroup.get('createdDate') as FormControl;
  }

  news: any[] = [];

  constructor(
    private route: Router,
    private newsService: NewsService,
    private tokenService: TokenService
  ) {}

  ngOnInit() {
    this.getNews();
    this.loggedInUserId = this.tokenService.getUserId() ?? 0;
  }

  getNews() {
    this.newsService.getNews().subscribe((data: any) => {
      this.news = data;
      this.filteredNews = [...this.news];
    });
  }

  onSearchChange(NewsSearchTerm: string) {
    const searchTerm = NewsSearchTerm.trim().toLowerCase();
    this.filteredNews = this.news.filter((news) => {
      return (
        news.title.toLowerCase().includes(searchTerm) ||
        news.shortDescription.toLowerCase().includes(searchTerm) ||
        news.content.toLowerCase().includes(searchTerm)
      );
    });
  }

  openCreateNews() {
    this.showNewsModal = true;
  }

  closeCreateNewsModel() {
    this.showNewsModal = false;
    this.newsFormGroup.reset();
  }

  openDeleteModal(news: any) {
    this.newsToDelete = news;
    this.showDeleteModal = true;
  }

  onNewsImageSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.newsFormGroup.patchValue({ image: file });
      this.newsFormGroup.get('image')?.updateValueAndValidity();
    }
  }

  saveNews() {
    const formData = new FormData();
    formData.append('UserId', this.loggedInUserId.toString());
    formData.append('Title', this.newsFormGroup.value.title ?? '');
    formData.append(
      'ShortDescription',
      this.newsFormGroup.value.shortDescription ?? ''
    );
    formData.append('Content', this.newsFormGroup.value.content ?? '');
    const createdDateStr = new Date(
      this.newsFormGroup.value.createdDate + 'T00:00:00'
    ).toISOString();
    formData.append('CreatedDate', createdDateStr ?? '');
    formData.append('Image', this.newsFormGroup.value.image ?? '');
    formData.append('Status', this.newsFormGroup.value.status ?? '');

    this.newsService.addNews(formData).subscribe({
      next: (response) => {
        this.closeCreateNewsModel();
        this.toast.display('News Created Successfully', 'success');
        this.getNews();
        this.newsFormGroup.reset();
      },
      error: (error) => {
        console.log(formData);
        console.error('Error saving news:', error);
      },
    });
  }

  editNews(news: any) {
    this.editNewsItem = news;
    this.showEditModal = true;

    this.newsFormGroup.patchValue({
      title: news.title,
      shortDescription: news.shortDescription,
      content: news.content,
      createdDate: news.createdDate?.split('T')[0],
      status: news.status,
      image: '',
    });
  }

  updateNews() {
    const formData = new FormData();
    formData.append('NewsId', this.editNewsItem.newsId.toString());
    formData.append('UserId', this.loggedInUserId.toString());
    formData.append('Title', this.newsFormGroup.value.title ?? '');
    formData.append(
      'ShortDescription',
      this.newsFormGroup.value.shortDescription ?? ''
    );
    formData.append('Content', this.newsFormGroup.value.content ?? '');
    const createdDateStr = new Date(
      this.newsFormGroup.value.createdDate + 'T00:00:00'
    ).toISOString();
    formData.append('CreatedDate', createdDateStr ?? '');
    if (this.newsFormGroup.value.image) {
      formData.append('Image', this.newsFormGroup.value.image);
    }
    formData.append('Status', this.newsFormGroup.value.status ?? '');

    this.newsService
      .updateNews(parseInt(this.editNewsItem.newsId), formData)
      .subscribe({
        next: () => {
          console.log('News updated successfully', formData);
          this.toast.display('News updated successfully', 'success');
          this.closeEditModal();
          this.getNews();
        },
        error: (err) => {
          console.error('Error updating news:', err);
        },
      });
  }

  closeEditModal() {
    this.showEditModal = false;
    this.newsFormGroup.reset();
    this.editNewsItem = null;
  }

  confirmDeleteNews() {
    this.newsService.deleteNews(this.newsToDelete.newsId).subscribe({
      next: () => {
        this.closeDeleteModel();
        this.toast.display('News deleted successfully', 'success');
      },
      error: (err) => {
        console.error('Error deleting news:', err);
      },
    });
  }

  closeDeleteModel() {
    this.showDeleteModal = false;
    this.newsToDelete = null;
  }

  changePage(delta: number) {
    this.filters.pageNumber += delta;
    if (this.filters.pageNumber < 1) this.filters.pageNumber = 1;
    this.getNews();
  }

  DownloadExcel() {
    this.newsService.exportToExcel().subscribe({
      next: (response: Blob) => {
        const blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'NewsExport.xlsx'; 
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        console.error('Download Excel failed', err);
      },
    });
  }

  DownloadCSV() {
    this.newsService.exportToCSV().subscribe({
      next: (response: Blob) => {
        const blob = new Blob([response], { type: 'text/csv' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'NewsExport.csv';
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        console.error('Download CSV failed', err);
      },
    });
  }
}
