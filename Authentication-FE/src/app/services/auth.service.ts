import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'https://localhost:7153/api';
  private path = 'auth';

  constructor(private http: HttpClient, private router: Router) { }

  // 1. Sign Up
  register(model: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/${this.path}/signup`, model);
  }

  // 2. Login
  login(model: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/${this.path}/login`, model).pipe(
      tap(response => {
        if (response.token) {
          localStorage.setItem('token', response.token);
        }
      })
    );
  }

  // 3. Log out
  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }

  // 4. Check Login
  loggedIn(): boolean {
    const token = localStorage.getItem('token');

    // 1. Check if a token is present
    if (!token) {
      return false;
    }

    try {
      // 2. Decode to view the payload
      const decodedToken: any = jwtDecode(token);

      // 3. Convert exp (seconds) to milliseconds
      const expirationDate = decodedToken.exp * 1000;

      // 4. Check: If the expiration time > current time, it has not expired.
      const isTokenValid = expirationDate > Date.now();

      // If token has expired, delete the token.
      if (!isTokenValid) {
        this.logout();
      }

      return isTokenValid;

    } catch (error) {
      // 5. If the token is malformed or cannot be decoded.
      console.error('Token is invalid or corrupted. Logging out.');
      this.logout();
      return false;
    }
  }

  public getProfile(): Observable<any> {
    return this.http.get(`${this.baseUrl}/${this.path}/profile`);
  }

}
