import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApprovalService {

  private apiUrl =
    'https://localhost:7258/api/Approval';

  constructor(private http: HttpClient) {}

  approve(
    approvalRequestId: string,
    reviewerId: string,
    reviewerComment: string
  ): Observable<any> {

    return this.http.post<any>(
      `${this.apiUrl}/${approvalRequestId}/approve`,
      {
        reviewerId,
        reviewerComment
      }
    );
  }

  reject(
    approvalRequestId: string,
    reviewerId: string,
    reviewerComment: string
  ): Observable<any> {

    return this.http.post<any>(
      `${this.apiUrl}/${approvalRequestId}/reject`,
      {
        reviewerId,
        reviewerComment
      }
    );
  }
}