import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

import {
  AuthService
} from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ CommonModule,
  FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {

  username = 'adjuster.a';
  password = 'Adjuster123!';

  loading = false;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  login(): void {

    this.loading = true;
    this.errorMessage = '';

    this.authService.login({
      username: this.username,
      password: this.password
    }).subscribe({

      next: () => {
        this.loading = false;
        this.router.navigate(['/']);
      },

      error: (error) => {

        this.loading = false;

        console.error(error);

        this.errorMessage =
          'Invalid username or password.';
      }

    });
  }
}