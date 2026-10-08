import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface LoginRequest {
  username: string;
  password: string;
  tenantId: string;
}

export interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = 'https://localhost:7258/api/Auth';

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<LoginResponse> {

    const headers = new HttpHeaders({
      'X-Tenant-Id': request.tenantId
    });

    return this.http.post<LoginResponse>(
      `${this.apiUrl}/login`,
      {
        username: request.username,
        password: request.password
      },
      { headers }
    ).pipe(
      tap(response => {
        localStorage.setItem('token', response.token);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}