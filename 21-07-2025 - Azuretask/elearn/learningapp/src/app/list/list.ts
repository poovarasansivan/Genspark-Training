import { ChangeDetectorRef, Component } from '@angular/core';
import { LucideAngularModule, BookOpenText } from 'lucide-angular';
import { Router, RouterLink } from '@angular/router';
import { VideoService } from '../services/VideoService';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-list',
  standalone: true,
  imports: [LucideAngularModule, RouterLink, CommonModule, FormsModule],
  templateUrl: './list.html',
  styleUrl: './list.css',
})
export class List {
  bookIcon = BookOpenText;
  videos: any[] = [];
  filteredVideos: any[] = [];
  searchTerm: string = '';
  loading = false;

  constructor(
    private router: Router,
    private videoService: VideoService,
    private cdr: ChangeDetectorRef
  ) {
    this.loadvideos();
  }

  loadvideos() {
    this.loading = true;
    this.videoService.getAllVideos().subscribe({
      next: (data) => {
        this.videos = data;
        this.loading = false;
        console.log('Videos loaded:', this.videos);
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error fetching videos:', error);
        this.loading = false;
      },
    });
  }

  onCourseClick(course: any) {
    this.router.navigate(['/details', course.id]);
  }

  onSearchChange() {
    if (this.searchTerm) {
      this.filteredVideos = this.videos.filter((video) =>
        video.title.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    } else {
      this.filteredVideos = this.videos;
    }
  }
}
