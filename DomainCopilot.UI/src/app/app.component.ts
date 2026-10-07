import { Component } from '@angular/core';

import {
  RouterLink,
  RouterOutlet,
  Router
} from '@angular/router';

import {
  AuthService
} from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  logout(): void {

    this.authService.logout();

    this.router.navigate(['/login']);
  }
}