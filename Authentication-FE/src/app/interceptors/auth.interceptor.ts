import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor{
  constructor(private authService: AuthService) { } 

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token = localStorage.getItem('token');

    // 1. push Token inside Header
    if (token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    // 2. send Request and check Error found
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        // 🚨 if Error Code 401 (Unauthorized)
        if (error.status === 401) {
          console.error('Interceptor: Token is invalid or expired. Logging out.');
          
          this.authService.logout(); 
          return throwError(() => new Error('Session Expired or Unauthorized'));
        }
        
        // if not 401 (400, 500, Network Error other)
        return throwError(() => error); 
      })
    );
  }

}
