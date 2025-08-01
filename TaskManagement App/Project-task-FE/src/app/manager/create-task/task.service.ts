import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { TaskFileDto, TaskItemRequestDto, TaskItemResponseDto, TaskItemUpdateDto, TaskStatusLogResponseDto } from './task.types';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResponse } from '../Create-User/user.types';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  private readonly baseUrl = 'http://localhost:5093/api/TaskItem';

  constructor(private http: HttpClient) {}

  createTask(dto: TaskItemRequestDto): Observable<TaskItemResponseDto> {
    const formData = new FormData();
    formData.append('Title', dto.title);
    formData.append('Description', dto.description || '');
    formData.append('Status', dto.status);
    if (dto.dueDate) formData.append('DueDate', dto.dueDate);
    formData.append('AssignedToId', dto.assignedToId);

    return this.http.post<TaskItemResponseDto>(this.baseUrl, formData);
  }

  draftTask(dto: TaskItemRequestDto): Observable<TaskItemResponseDto> {
    const formData = new FormData();
    formData.append('Title', dto.title);
    formData.append('Description', dto.description || '');
    formData.append('Status', dto.status|| 'Draft'); 
    if (dto.dueDate) formData.append('DueDate', dto.dueDate|| '');
    formData.append('AssignedToId', dto.assignedToId|| '');
    return this.http.post<TaskItemResponseDto>(`${this.baseUrl}/draft`, formData);
  }

getAllTasks(
  page: number,
  pageSize: number,
  status: string = '',
  title: string = '',
  dueDate: string = ''
): Observable<PagedResponse<TaskItemResponseDto>> {
  const params: any = {
    page,
    pageSize,
  };

  if (status) params.status = status;
  if (title) params.title = title;
  if (dueDate) params.dueDate = dueDate;

  return this.http.get<PagedResponse<TaskItemResponseDto>>(this.baseUrl, { params });
}


updateTask(id: string, dto: TaskItemUpdateDto): Observable<void> {
  return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
}

deleteTask(id: string): Observable<any> {
  return this.http.delete(`${this.baseUrl}/${id}`);
}

uploadFile(taskId: string, formData: FormData): Observable<any> {
  return this.http.post(`http://localhost:5093/api/TaskFile/${taskId}/upload`, formData);
}

getFiles(taskId: string): Observable<ApiResponse<TaskFileDto[]>> {
  return this.http.get<ApiResponse<TaskFileDto[]>>(`http://localhost:5093/api/TaskFile/${taskId}/files`);
}

downloadFile(fileId: string): Observable<Blob> {
  return this.http.get(`http://localhost:5093/api/TaskFile/download/${fileId}`, {
    responseType: 'blob'
  });
}

searchTasks(title: string, dueDate: string): Observable<ApiResponse<TaskItemResponseDto[]>> {
  const params = new HttpParams()
    .set('title', title || '')
    .set('dueDate', dueDate || '');

  return this.http.get<ApiResponse<TaskItemResponseDto[]>>(
    `${this.baseUrl}/search`, { params }
  );
}

getTaskStatusLogs(taskId: string) {
  return this.http.get<ApiResponse<TaskStatusLogResponseDto[]>>(`http://localhost:5093/api/TaskStatusLog/task/${taskId}`);
}

}
