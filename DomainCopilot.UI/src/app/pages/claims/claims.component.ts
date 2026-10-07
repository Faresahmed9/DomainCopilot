import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import {
  ClaimsService,
  ClaimSummary
} from '../../core/services/claims.service';

@Component({
  selector: 'app-claims',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './claims.component.html',
  styleUrl: './claims.component.scss'
})
export class ClaimsComponent implements OnInit {

  claims: ClaimSummary[] = [];

  loading = false;
  errorMessage = '';

  constructor(
    private claimsService: ClaimsService,
    private router: Router
  ) {}

  ngOnInit(): void {

    this.loading = true;

    this.claimsService.getClaims().subscribe({

      next: data => {
        this.claims = data;
        this.loading = false;
      },

      error: error => {

        console.error(error);

        this.loading = false;
        this.errorMessage =
          'Unable to load claims.';
      }

    });
  }

  openClaim(claimId: string): void {

    this.router.navigate([
      '/claims',
      claimId
    ]);
  }
}