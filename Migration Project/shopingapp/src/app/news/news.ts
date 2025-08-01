import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { NewsService } from '../services/news-service';

@Component({
  selector: 'app-news',
  imports: [CommonModule],
  templateUrl: './news.html',
  styleUrl: './news.css',
})
export class News {
  posts: any[] = [];
  constructor(private route: Router, private newsService: NewsService) {}

  ngOnInit() {
    this.getNews();
  }

  getNews() {
    this.newsService.getNews().subscribe(
      (data: any) => {
        this.posts = data;
      },
      (error: any) => {
        console.error('Error fetching news:', error);
      }
    );
  }

  onPostClick(post: any) {
    this.route.navigate(['/blog-details', post.newsId]);
  }
}
