import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ClaimSummary {
  claimId: string;
  claimNumber: string;
  policyNumber: string;
  incidentDate: string;
  claimedAmount: number;
}

@Injectable({
  providedIn: 'root'
})
export class ClaimsService {

  private apiUrl =
    'https://localhost:7258/api/Claims';

  constructor(private http: HttpClient) {}

  getClaims(): Observable<ClaimSummary[]> {

    return this.http.get<ClaimSummary[]>(
      this.apiUrl
    );
  }

  getClaimContext(
    claimId: string
  ): Observable<any> {

    return this.http.get<any>(
      `${this.apiUrl}/${claimId}/context`
    );
  }

  orchestrate(
    claimId: string
  ): Observable<any> {

    return this.http.post<any>(
      `${this.apiUrl}/${claimId}/orchestrate`,
      {}
    );
  }
}