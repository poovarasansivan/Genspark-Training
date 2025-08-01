import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NewsService } from '../services/news-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-news-details',
  imports: [CommonModule,FormsModule],
  templateUrl: './news-details.html',
  styleUrl: './news-details.css',
})
export class NewsDetails {
  BlogPost: any;
  constructor(
    private newsService: NewsService,
    private route: ActivatedRoute
  ) {}
  ngOnInit() {
    this.getNewsDetails();
  }
  getNewsDetails() {
    const newsId = parseInt(this.route.snapshot.paramMap.get('id') || '', 10);
    if (newsId) {
      this.newsService.getNewsById(newsId).subscribe(
        (data: any) => {
          this.BlogPost = data;
        },
        (error: any) => {
          console.error('Error fetching news details:', error);
        }
      );
    } else {
      console.error('No news ID found in route parameters.');
    }
  }
}
