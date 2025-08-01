import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { Router } from '@angular/router';
import { ApiResponse, LoginResponseDto } from './auth.types';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let routerSpy: jasmine.SpyObj<Router>;
  const baseUrl = 'http://localhost:5093/api/v1/auth';

  beforeEach(() => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        { provide: Router, useValue: routerSpy }
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    sessionStorage.clear();
    document.cookie = '';
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should login and store session data', () => {
    const mockResponse: ApiResponse<LoginResponseDto> = {
      success: true,
      message: 'Logged in',
      data: {
        accessToken: 'access-token',
        refreshToken: 'refresh-token',
        user: {
          id: '1',
          fullName: 'Test User',
          email: 'test@example.com',
          role: 'Manager'
        },
        expiresIn: 3600
      },
      errors: {}
    };

    service.login({ email: 'test@example.com', password: '123456' }).subscribe();

    const req = httpMock.expectOne(`${baseUrl}/login`);
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);

    expect(sessionStorage.getItem('accessToken')).toBe('access-token');
    expect(sessionStorage.getItem('user')).toContain('Test User');
    expect(document.cookie).toContain('refreshToken=refresh-token');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

    it('should refresh token and update session', () => {
    spyOn(service as any, 'getRefreshToken').and.returnValue('refresh-token');

    const mockResponse: ApiResponse<LoginResponseDto> = {
      success: true,
      message: 'Refreshed',
      data: {
        accessToken: 'new-access-token',
        refreshToken: 'new-refresh-token',
        user: {
          id: '1',
          fullName: 'Test User',
          email: 'test@example.com',
          role: 'Manager'
        },
        expiresIn: 3600
      },
      errors: {}
    };

    service.refreshToken().subscribe(success => {
      expect(success).toBeTrue();
      expect(sessionStorage.getItem('accessToken')).toBe('new-access-token');
    });

    const req = httpMock.expectOne(`${baseUrl}/refresh`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ refreshToken: 'refresh-token' });
    req.flush(mockResponse);
  });


  it('should return false if no refresh token is present', () => {
    service.refreshToken().subscribe(success => {
      expect(success).toBeFalse();
    });
  });


  it('should still clean session if logout API fails', () => {
    sessionStorage.setItem('accessToken', 'token');
    sessionStorage.setItem('user', JSON.stringify({ id: '1' }));

    spyOn(service as any, 'getRefreshToken').and.returnValue('test-token');

    service.logout();

    const req = httpMock.expectOne(`${baseUrl}/logout`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ refreshToken: 'test-token' });

    req.error(new ErrorEvent('Network error'));

    expect(sessionStorage.getItem('accessToken')).toBeNull();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });


  it('should return userId from token', () => {
    const payload = {
      nameid: '123',
      sub: 'test@example.com'
    };
    const token = `header.${btoa(JSON.stringify(payload))}.signature`;
    sessionStorage.setItem('accessToken', token);

    const userId = service.getUserId();
    expect(userId).toBe('123');
  });

  it('should return empty string for invalid token', () => {
    sessionStorage.setItem('accessToken', 'invalid.token');
    const userId = service.getUserId();
    expect(userId).toBe('');
  });

  it('should return user object from session', () => {
    const user = { id: '1', role: 'Manager' };
    sessionStorage.setItem('user', JSON.stringify(user));
    expect(service.getUser()).toEqual(user);
  });

  it('should return null when no user in session', () => {
    expect(service.getUser()).toBeNull();
  });

  it('should detect authentication status', () => {
    expect(service.isAuthenticated()).toBeFalse();
    sessionStorage.setItem('accessToken', 'token');
    expect(service.isAuthenticated()).toBeTrue();
  });
});
