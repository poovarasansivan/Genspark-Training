import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserRequestDto, UserResponseDto ,ApiResponse, PagedResponse, UserUpdateDto } from '../Create-User/user.types';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = 'http://localhost:5093/api/v1/users';

  constructor(private http: HttpClient) {}

  createUser(dto: UserRequestDto): Observable<ApiResponse<UserResponseDto>> {
    return this.http.post<ApiResponse<UserResponseDto>>(this.baseUrl, dto);
  }

   getAllUsers(
    page: number = 1,
    pageSize: number = 10,
    search: string = '',
    role: string = ''
  ): Observable<PagedResponse<UserResponseDto>> {
    return this.http.get<PagedResponse<UserResponseDto>>(`${this.baseUrl}`, {
      params: {
        page,
        pageSize,
        search,
        role
      }
    });
  }

  getUserById(id: string): Observable<UserResponseDto> {
    return this.http.get<UserResponseDto>(`${this.baseUrl}/${id}`);
  }

  updateUser(id: string, dto: UserUpdateDto): Observable<UserResponseDto> {
    return this.http.put<UserResponseDto>(`${this.baseUrl}/${id}`, dto);
  }

  deleteUser(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  getAllTeamMembers(): Observable<ApiResponse<UserResponseDto[]>> {
    return this.http.get<ApiResponse<UserResponseDto[]>>(`${this.baseUrl}/team-members`);
  }
}
