import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import {
  ClaimsService
} from '../../core/services/claims.service';

@Component({
  selector: 'app-claim-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './claim-details.component.html',
  styleUrl: './claim-details.component.scss'
})
export class ClaimDetailsComponent implements OnInit {

  claimId = '';

  claim: any = null;

  loading = false;
  orchestrating = false;

  progressMessages: string[] = [];

  errorMessage = '';
  approvalRequestId = '';

  analysisCompleted = false;

  constructor(
    private route: ActivatedRoute,
    private claimsService: ClaimsService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.claimId =
      this.route.snapshot.paramMap.get('id') || '';

    this.loadClaim();
  }

  goToApproval(): void {

    if (!this.approvalRequestId) {
      return;
    }

    this.router.navigate(
      ['/approvals'],
      {
        queryParams: {
          approvalRequestId:
            this.approvalRequestId
        }
      }
    );
  }

  loadClaim(): void {

this.loading = true;
this.errorMessage = '';

this.claimsService
.getClaimContext(this.claimId)
.subscribe({


  next: data => {

    this.claim = {
      ...data.claim,
      policy: data.policy,
      coverages: data.coverages,
      exclusions: data.exclusions
    };

    this.loading = false;
  },

  error: error => {

    console.error(error);

    this.loading = false;

    this.errorMessage =
      'Unable to load claim.';
  }

});


}


  runOrchestration(): void {

    this.orchestrating = true;
    this.errorMessage = '';
    this.analysisCompleted = false;
    this.approvalRequestId = '';

    this.progressMessages = [
      'Retrieving policy...',
      'Checking coverage...',
      'Analyzing exclusions...',
      'Detecting anomalies...',
      'Calculating approved amount...',
      'Creating approval request...'
    ];

    this.claimsService
      .orchestrate(this.claimId)
      .subscribe({

        next: result => {

          this.claim = {
            ...this.claim,
            ...result
          };

          this.approvalRequestId =
            result.approvalRequestId || '';

          this.analysisCompleted = true;
          this.orchestrating = false;

        },

        error: error => {

          console.error(error);

          this.orchestrating = false;

          this.errorMessage =
            'AI orchestration failed.';
        }

      });
  }
}