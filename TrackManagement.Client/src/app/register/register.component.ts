import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
  standalone: false
})
export class RegisterComponent {
  username = '';
  password = '';
  role = 'Operator';
  readonly roles = ['Admin', 'Operator', 'Editor', 'Viewer'];
  showPassword = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  isSubmitting = false;

  constructor(private authService: AuthService) {}

  get canSubmit(): boolean {
    return this.username.trim().length >= 3 && this.password.length >= 8 && !!this.role && !this.isSubmitting;
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  submit(): void {
    this.errorMessage = null;
    this.successMessage = null;
    this.isSubmitting = true;

    this.authService.register(this.username, this.password, this.role).subscribe({
      next: (createdUser) => {
        this.isSubmitting = false;
        this.successMessage = `${createdUser.role} user added successfully.`;
        this.username = '';
        this.password = '';
        this.role = 'Operator';
        this.showPassword = false;
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = err?.error?.detail ?? 'Could not add the user.';
      }
    });
  }
}
