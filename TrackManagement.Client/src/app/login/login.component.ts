import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: false
})
export class LoginComponent {
  username = 'admin';
  password = 'Password123@#';
  errorMessage: string | null = null;
  isSubmitting = false;

  constructor(private authService: AuthService, private router: Router) {}

  submit(): void {
    this.errorMessage = null;
    this.isSubmitting = true;

    this.authService.login(this.username, this.password).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/tracks']);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = err?.error?.detail ?? 'Login failed. Check your credentials.';
      }
    });
  }
}
