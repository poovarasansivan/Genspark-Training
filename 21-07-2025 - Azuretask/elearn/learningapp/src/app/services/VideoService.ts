import { HttpClient, HttpHeaders } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { UploadVideoDto } from "../models/videomodel";

@Injectable({ providedIn: "root" })
export class VideoService {
    
  private baseUrl: string = "http://localhost:5283/api";
  private accessToken: string | null = localStorage.getItem("token");
  private http = inject(HttpClient);

  uploadVideo(videoData: any): Observable<any> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${this.accessToken}`,
    });
    return this.http.post<any>(`${this.baseUrl}/videos/upload`, videoData, { headers });
  }

  getAllVideos(): Observable<any> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${this.accessToken}`,
      "Content-Type": "application/json",
    });
    return this.http.get<any>(`${this.baseUrl}/videos`, { headers }).pipe(
      map((response: any) => {
        const values = response?.$values ?? [];
        return values;
      })
    );
  }

  getVideoById(videoId: string): Observable<any> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${this.accessToken}`,
      "Content-Type": "application/json",
    });
    return this.http.get<any>(`${this.baseUrl}/videos/${videoId}`, { headers }).pipe(
      map((response: any) => {
        return response ?? null;
      })
    );
  }
}