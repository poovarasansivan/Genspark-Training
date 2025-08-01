export interface UserRequestDto {
  fullName: string;
  email: string;
  password: string;
  role: 'Manager' | 'TeamMember'; // based on your backend enum
}

export interface UserResponseDto {
  id: string;
  fullName: string;
  email: string;
  role: 'Manager' | 'TeamMember';
  createdAt: string;
  updatedAt?: string;
}


export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors: { [key: string]: string[] };
}



export interface UserUpdateDto {
  fullName?: string;
  email?: string;
  password?: string;
  role?: string;
}

export interface PaginationMetadata {
  totalRecords: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface PagedResponse<T> {
  data: T[];
  message: string;
  pagination: PaginationMetadata;
}
