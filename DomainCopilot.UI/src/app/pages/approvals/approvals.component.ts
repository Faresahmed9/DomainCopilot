import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import {
  ApprovalService
} from '../../core/services/approval.service';

@Component({
  selector: 'app-approvals',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './approvals.component.html',
  styleUrl: './approvals.component.scss'
})
export class ApprovalsComponent implements OnInit {

  approvalRequestId = '';

  reviewerId = 'adjuster.a';

  comment =
    'Reviewed claim and AI recommendation.';

  message = '';

  loading = false;

  constructor(
    private approvalService: ApprovalService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {

    this.route.queryParams.subscribe(params => {

      const requestId =
        params['approvalRequestId'];

      if (requestId) {
        this.approvalRequestId = requestId;
      }

    });

  }

  approve(): void {

    if (!this.approvalRequestId) {

      this.message =
        'Enter an approval request ID first.';

      return;
    }

    this.loading = true;

    this.approvalService
      .approve(
        this.approvalRequestId,
        this.reviewerId,
        this.comment
      )
      .subscribe({

        next: () => {

          this.loading = false;

          this.message =
            'Claim approved successfully.';
        },

        error: error => {

          console.error(error);

          this.loading = false;

          this.message =
            'Approval failed.';
        }

      });
  }

  reject(): void {

    if (!this.approvalRequestId) {

      this.message =
        'Enter an approval request ID first.';

      return;
    }

    this.loading = true;

    this.approvalService
      .reject(
        this.approvalRequestId,
        this.reviewerId,
        this.comment
      )
      .subscribe({

        next: () => {

          this.loading = false;

          this.message =
            'Claim rejected successfully.';
        },

        error: error => {

          console.error(error);

          this.loading = false;

          this.message =
            'Rejection failed.';
        }

      });
  }
}