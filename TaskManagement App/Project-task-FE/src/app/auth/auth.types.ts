export interface LoginRequestDto {
  email: string;
  password: string;
}

export interface MeResponseDto {
  id: string;
  fullName: string;
  email: string;
  role: string;
}

export interface LoginResponseDto {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: MeResponseDto;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors: { [key: string]: string[] };
}
