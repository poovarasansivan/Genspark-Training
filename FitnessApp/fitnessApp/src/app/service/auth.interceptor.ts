import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { BehaviorSubject, catchError, filter, Observable, switchMap, take, throwError } from "rxjs";
import { TokenService } from "./token.service";
import { AuthService } from "./auth.service";
import { Injectable } from "@angular/core";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshSubject = new BehaviorSubject<string | null>(null);

  constructor(private tokenService: TokenService, private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const accessToken = this.tokenService.getAccessToken();

    let authReq = req;

    if (accessToken) {
      authReq = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${accessToken}`),
      });
    }

    return next.handle(authReq).pipe(
      catchError(err => {

        if (err.status === 401 && !authReq.url.includes('refresh')) {
          // console.log('🔁 Token likely expired. Triggering refresh...');
          return this.handle401(authReq, next);
        }

        return throwError(() => err);
      })
    );
  }

  private handle401(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      // console.log('🔄 Refreshing access token...');

      this.isRefreshing = true;
      this.refreshSubject.next(null);

      return this.authService.refreshToken().pipe(
        switchMap(res => {
          this.isRefreshing = false;
          const newAccessToken = res.accessToken;
          const newRefreshToken = res.refreshToken;

          // console.log('✅ New Access Token received:', newAccessToken);
          // console.log('✅ New Refresh Token received:', newRefreshToken);

          this.tokenService.saveTokens(newAccessToken, newRefreshToken);
          // console.log('💾 Tokens saved to localStorage');

          this.refreshSubject.next(newAccessToken);

          return next.handle(
            req.clone({
              headers: req.headers.set('Authorization', `Bearer ${newAccessToken}`),
            })
          );
        }),
        catchError(err => {
          // console.error('🛑 Failed to refresh token:', err);
          this.isRefreshing = false;
          this.tokenService.clearTokens();
          return throwError(() => err);
        })
      );
    } else {
      // console.log('⏳ Already refreshing. Queuing request...');

      return this.refreshSubject.pipe(
        filter(token => token != null),
        take(1),
        switchMap(token =>
          next.handle(
            req.clone({
              headers: req.headers.set('Authorization', `Bearer ${token}`),
            })
          )
        )
      );
    }
  }
}
