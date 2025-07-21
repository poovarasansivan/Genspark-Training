import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { BookOpenText, LucideAngularModule } from 'lucide-angular';
import { VideoService } from '../services/VideoService';

@Component({
  selector: 'app-form',
  imports: [RouterLink, LucideAngularModule, CommonModule, FormsModule],
  templateUrl: './form.html',
  styleUrl: './form.css',
})
export class Form {
  bookIcon = BookOpenText;
  video = {
    title: '',
    description: '',
    uploadDate: '',
    category: '',
  };

  videoFile: File | null = null;
  thumbnailFile: File | null = null;

  constructor(private router: Router, private videoService:VideoService) {}
   onVideoFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input?.files?.length) {
      this.videoFile = input.files[0];
    }
  }

  onThumbnailFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input?.files?.length) {
      this.thumbnailFile = input.files[0];
    }
  }

  onSubmit() {
    if (!this.videoFile || !this.thumbnailFile) {
      console.error('Please select both video and thumbnail files.');
      return;
    }

    const date = new Date(this.video.uploadDate + 'T00:00:00Z');

    const formData = new FormData();
    formData.append('videoFile', this.videoFile);
    formData.append('thumbnailFile', this.thumbnailFile);
    formData.append('title', this.video.title);
    formData.append('description', this.video.description);
    formData.append('uploadDate', date.toISOString());
    formData.append('category', this.video.category);

    this.videoService.uploadVideo(formData).subscribe({
      next: (response) => {
        console.log('Video uploaded successfully:', response);
        this.router.navigate(['/']);
      },
      error: (error) => {
        console.error('Error uploading video:', error);
      }
    });
  }
  
}
