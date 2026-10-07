import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DashboardService } from '../../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {

  totalClaims = 0;
  pendingApprovals = 0;

  constructor(
    private dashboardService: DashboardService
  ) {}

  ngOnInit(): void {

    this.dashboardService.getSummary().subscribe({
      next: (data) => {
        this.totalClaims = data.totalClaims;
        this.pendingApprovals = data.pendingApprovals;
      },

      error: (error) => {
        console.error(
          'Failed to load dashboard summary',
          error
        );
      }
    });

  }
}