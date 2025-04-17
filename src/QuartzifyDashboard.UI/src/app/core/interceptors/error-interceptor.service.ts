import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../auth/auth.service';

@Injectable()

export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private router: Router,
    private toastr: ToastrService,
    private authService: AuthService
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'An unknown error occurred';
        
        if (error.error instanceof ErrorEvent) {
          // Client-side error
          errorMessage = `Error: ${error.error.message}`;
        } else {
          // Server-side error
          if (error.status === 401) {
            this.authService.logout();
            this.router.navigate(['/auth/login']);
            errorMessage = 'Your session has expired. Please log in again.';
          } else if (error.status === 403) {
            errorMessage = 'You do not have permission to access this resource.';
          } else if (error.error && error.error.message) {
            errorMessage = error.error.message;
          } else {
            errorMessage = `Error Code: ${error.status}, Message: ${error.message}`;
          }
        }
        
        this.toastr.error(errorMessage, 'Error');
        return throwError(() => error);
      })
    );
  }
}