import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DashboardSummary {
  totalClaims: number;
  pendingApprovals: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

 private apiUrl = 'https://localhost:7258/api/Dashboard';

  constructor(private http: HttpClient) {}

  getSummary(): Observable<DashboardSummary> {
    return this.http.get<DashboardSummary>(
      `${this.apiUrl}/summary`
    );
  }
}