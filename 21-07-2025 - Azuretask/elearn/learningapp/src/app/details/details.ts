import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { LucideAngularModule, BookOpenText } from 'lucide-angular';
import { VideoService } from '../services/VideoService';
import { ActivatedRoute, RouterLink } from '@angular/router';

@Component({
  selector: 'app-details',
  imports: [LucideAngularModule, CommonModule, RouterLink],
  templateUrl: './details.html',
  styleUrl: './details.css',
})
export class Details {
  bookIcon = BookOpenText;
  video: any;
  loading = true;
  constructor(
    private route: ActivatedRoute,
    private videoService: VideoService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    const videoId = this.route.snapshot.paramMap.get('id');
    this.loading = true;

    if (videoId) {
      this.videoService.getVideoById(videoId).subscribe({
        next: (data) => {
          this.video = data;
          this.loading = false;
          this.cdr.detectChanges(); // manually trigger change detection
        },
        error: (error) => {
          console.error('Error fetching video details:', error);
          this.loading = false;
        },
      });
    }
  }
}
