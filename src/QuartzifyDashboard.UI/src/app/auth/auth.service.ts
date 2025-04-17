import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

interface LoginRequest {
  username: string;
  password: string;
}

interface LoginResponse {
  token: string;
  expiresIn: number;
}

interface DecodedToken {
  sub: string;
  exp: number;
  role: string;
}

@Injectable({
  providedIn: 'root'
})

export class AuthService {
  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  private tokenExpirationTimer: any;

  constructor(private http: HttpClient, private router: Router) {
    this.checkAuthState();
  }

  public login(loginRequest: LoginRequest): Observable<boolean> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/quartzify/api/auth/login`, loginRequest)
      .pipe(
        tap(response => {
          this.handleAuthentication(response.token, response.expiresIn);
        }),
        map(() => true)
      );
  }

  public logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('expiration');
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);

    if (this.tokenExpirationTimer) {
      clearTimeout(this.tokenExpirationTimer);
    }
    this.tokenExpirationTimer = null;
  }

  public isAuthenticated(): boolean {
    return !!this.currentUserSubject.value;
  }

  public getToken(): string | null {
    return localStorage.getItem('token');
  }

  private checkAuthState(): void {
    const token = localStorage.getItem('token');
    const expirationDate = localStorage.getItem('expiration');

    if (!token || !expirationDate) {
      return;
    }

    const expirationTime = new Date(expirationDate).getTime() - new Date().getTime();

    if (expirationTime > 0) {
      const decodedToken = jwtDecode<DecodedToken>(token);
      this.currentUserSubject.next({
        username: decodedToken.sub,
        role: decodedToken.role
      });
      this.autoLogout(expirationTime);
    } else {
      this.logout();
    }
  }

  private handleAuthentication(token: string, expiresIn: number): void {
    const expirationDate = new Date(new Date().getTime() + expiresIn * 1000);

    localStorage.setItem('token', token);
    localStorage.setItem('expiration', expirationDate.toISOString());

    const decodedToken = jwtDecode<DecodedToken>(token);
    this.currentUserSubject.next({
      username: decodedToken.sub,
      role: decodedToken.role
    });

    this.autoLogout(expiresIn * 1000);
  }

  private autoLogout(expirationDuration: number): void {
    this.tokenExpirationTimer = setTimeout(() => {
      this.logout();
    }, expirationDuration);
  }
}