import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import {
  BehaviorSubject,
  catchError,
  filter,
  Observable,
  switchMap,
  take,
  throwError,
} from 'rxjs';
import { Injectable } from '@angular/core';
import { TokenService } from './token-service';
import { AuthService } from './auth-service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshSubject = new BehaviorSubject<string | null>(null);
  constructor(
    private tokenService: TokenService,
    private authService: AuthService
  ) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const accessToken = this.tokenService.getAccessToken();
    let authReq = request;
    if (accessToken) {
      authReq = request.clone({
        headers: request.headers.set('Authorization', `Bearer ${accessToken}`),
      });
    }

    return next.handle(request).pipe(
      catchError((error) => {
        if (error.status === 401 && !authReq.url.includes('refresh')) {
          return this.handle401Error(authReq, next);
        }
        return throwError(() => error);
      })
    );
  }

  private handle401Error(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshSubject.next(null);
      return this.authService.refreshToken().pipe(
        switchMap((token) => {
          this.isRefreshing = false;
          const newAccessToken = token.accessToken;
          const newRefreshToken = token.refreshToken;
          this.tokenService.setTokens(newAccessToken, newRefreshToken);
          this.refreshSubject.next(newAccessToken);
          return next.handle(
            request.clone({
              headers: request.headers.set(
                'Authorization',
                `Bearer ${newAccessToken}`
              ),
            })
          );
        }),
        catchError((err) => {
          this.isRefreshing = false;
          this.tokenService.clearTokens();
          return throwError(() => err);
        })
      );
    } else {
      return this.refreshSubject.pipe(
        filter((token) => token != null),
        take(1),
        switchMap((token) =>
          next.handle(
            request.clone({
              headers: request.headers.set('Authorization', `Bearer ${token}`),
            })
          )
        )
      );
    }
  }
}
