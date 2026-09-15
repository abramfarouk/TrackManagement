import { Component, OnInit } from '@angular/core';
import { Artist, CreateArtist } from '../models/models';
import { ArtistService } from '../services/artist.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-artist-list',
  templateUrl: './artist-list.component.html',
  styleUrls: ['./artist-list.component.css'],
  standalone: false
})
export class ArtistListComponent implements OnInit {
  artists: Artist[] = [];
  isLoading = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  showCreateDialog = false;
  isSubmitting = false;

  form: CreateArtist = {
    name: '',
    email: '',
    country: ''
  };

  formErrors: Record<string, string> = {};

  constructor(private artistService: ArtistService, public authService: AuthService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    if (!this.authService.isLoggedIn()) {
      this.artists = [];
      this.errorMessage = 'You must be signed in to access the artist list.';
      this.isLoading = false;
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    this.artistService.getAll().subscribe({
      next: (artists) => {
        if (this.authService.isLoggedIn()) {
          this.artists = artists;
          this.isLoading = false;
        }
      },
      error: () => {
        this.artists = [];
        this.errorMessage = 'We could not load the artist list. Please try again shortly.';
        this.isLoading = false;
      }
    });
  }

  openCreateDialog(): void {
    this.successMessage = null;
    this.errorMessage = null;
    this.formErrors = {};
    this.form = { name: '', email: '', country: '' };
    this.showCreateDialog = true;
  }

  closeCreateDialog(): void {
    this.showCreateDialog = false;
    this.formErrors = {};
    this.errorMessage = null;
    this.successMessage = null;
  }

  validateArtistField(field: keyof CreateArtist): void {
    const errors = this.validateForm();
    if (errors[field]) {
      this.formErrors[field] = errors[field];
    } else {
      delete this.formErrors[field];
    }
  }

  revalidateArtistField(field: keyof CreateArtist): void {
    if (this.formErrors[field]) {
      this.validateArtistField(field);
    }
  }

  submitArtist(): void {
    if (!this.authService.isLoggedIn()) {
      this.errorMessage = 'You must be signed in to manage artists.';
      return;
    }

    this.formErrors = this.validateForm();
    if (Object.keys(this.formErrors).length > 0) {
      this.errorMessage = 'Please review the highlighted fields and try again.';
      return;
    }

    this.isSubmitting = true;
    const payload: CreateArtist = {
      name: this.form.name.trim(),
      email: this.form.email.trim(),
      country: this.form.country.trim()
    };

    this.artistService.create(payload).subscribe({
      next: (artist) => {
        this.isSubmitting = false;
        this.closeCreateDialog();
        this.successMessage = `Artist “${artist.name}” was created successfully.`;
        this.load();
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = this.extractErrorMessage(err);
      }
    });
  }

  validateForm(): Record<string, string> {
    const errors: Record<string, string> = {};

    if (!this.form.name || this.form.name.trim().length < 2) {
      errors['name'] = 'Artist name is required.';
    }

    if (!this.form.email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.form.email.trim())) {
      errors['email'] = 'Enter a valid email address.';
    }

    if (!this.form.country || this.form.country.trim().length < 2) {
      errors['country'] = 'Country is required.';
    }

    return errors;
  }

  extractErrorMessage(err: any): string {
    const body = err?.error;
    if (body?.errors) {
      return Object.values(body.errors).flat().join(' ');
    }
    return body?.detail ?? 'Unable to create artist. Please review the form and try again.';
  }
}
