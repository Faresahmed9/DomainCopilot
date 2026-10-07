import { Routes } from '@angular/router';

import {
  DashboardComponent
} from './pages/dashboard/dashboard.component';

import {
  LoginComponent
} from './pages/login/login.component';

import {
  ClaimsComponent
} from './pages/claims/claims.component';

import {
  ClaimDetailsComponent
} from './pages/claim-details/claim-details.component';

import {
  ApprovalsComponent
} from './pages/approvals/approvals.component';

export const routes: Routes = [

  {
    path: 'login',
    component: LoginComponent
  },

  {
    path: '',
    component: DashboardComponent
  },

  {
    path: 'claims',
    component: ClaimsComponent
  },

  {
    path: 'claims/:id',
    component: ClaimDetailsComponent
  },

  {
    path: 'approvals',
    component: ApprovalsComponent
  },

  {
    path: '**',
    redirectTo: ''
  }

];