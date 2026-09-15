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
  username = '';
  password = '';
  showPassword = false;
  errorMessage: string | null = null;
  isSubmitting = false;

  constructor(private authService: AuthService, private router: Router) {}

  get canSubmit(): boolean {
    return this.username.trim().length > 0 && this.password.length > 0 && !this.isSubmitting;
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

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
        this.errorMessage = err?.error?.message ?? err?.error?.detail ?? 'Login failed. Check your credentials.';
      }
    });
  }
}
