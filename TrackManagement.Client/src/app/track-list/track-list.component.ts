import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { Artist, CreateTrack, Track, TrackStatus } from '../models/models';
import { TrackService } from '../services/track.service';
import { AuthService } from '../services/auth.service';
import { ArtistService } from '../services/artist.service';

@Component({
  selector: 'app-track-list',
  templateUrl: './track-list.component.html',
  styleUrls: ['./track-list.component.css'],
  standalone: false
})
export class TrackListComponent implements OnInit, OnDestroy {
  tracks: Track[] = [];
  artists: Artist[] = [];
  statusFilter = '';
  readonly statuses: TrackStatus[] = ['Draft', 'Submitted', 'Distributed'];

  isLoading = false;
  isSubmitting = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  showCreateDialog = false;
  private authSubscription?: Subscription;
  private trackRequestId = 0;
  private artistRequestId = 0;

  form: CreateTrack = {
    title: '',
    artistId: 0,
    isrc: '',
    releaseDate: new Date().toISOString().substring(0, 10),
    genre: ''
  };

  formErrors: Record<string, string> = {};

  constructor(
    private trackService: TrackService,
    private artistService: ArtistService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.authSubscription = this.authService.isLoggedIn$.subscribe(() => {
      this.refreshAccessState();
    });
  }

  ngOnDestroy(): void {
    this.authSubscription?.unsubscribe();
  }

  refreshAccessState(): void {
    if (!this.authService.isLoggedIn()) {
      this.trackRequestId++;
      this.artistRequestId++;
      this.tracks = [];
      this.artists = [];
      this.isLoading = false;
      this.isSubmitting = false;
      this.showCreateDialog = false;
      this.formErrors = {};
      this.errorMessage = 'You must be signed in to access the music library.';
      this.successMessage = null;
      return;
    }

    this.errorMessage = null;
    this.load();
    this.loadArtists();
  }

  load(): void {
    if (!this.authService.isLoggedIn()) {
      this.trackRequestId++;
      this.tracks = [];
      this.isLoading = false;
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;
    const requestId = ++this.trackRequestId;

    this.trackService.getAll({ status: this.statusFilter || null }).subscribe({
      next: (tracks) => {
        if (!this.authService.isLoggedIn() || requestId !== this.trackRequestId) {
          return;
        }
        this.tracks = tracks;
        this.isLoading = false;
      },
      error: () => {
        if (!this.authService.isLoggedIn() || requestId !== this.trackRequestId) {
          return;
        }
        this.tracks = [];
        this.errorMessage = 'Could not load tracks. Is the API running?';
        this.isLoading = false;
      }
    });
  }

  loadArtists(): void {
    if (!this.authService.isLoggedIn()) {
      this.artists = [];
      return;
    }

    const requestId = ++this.artistRequestId;

    this.artistService.getAll().subscribe({
      next: (artists) => {
        if (this.authService.isLoggedIn() && requestId === this.artistRequestId) {
          this.artists = artists;
        }
      },
      error: () => {
        if (this.authService.isLoggedIn() && requestId === this.artistRequestId) {
          this.errorMessage = 'Could not load artists. Please refresh and try again.';
        }
      }
    });
  }

  onFilterChange(): void {
    this.load();
  }

  openCreateDialog(): void {
    this.successMessage = null;
    this.errorMessage = null;
    this.formErrors = {};
    this.form = {
      title: '',
      artistId: 0,
      isrc: '',
      releaseDate: new Date().toISOString().substring(0, 10),
      genre: ''
    };
    this.showCreateDialog = true;
  }

  closeCreateDialog(): void {
    this.showCreateDialog = false;
    this.formErrors = {};
    this.errorMessage = null;
    this.successMessage = null;
  }

  validateTrackField(field: keyof CreateTrack): void {
    const errors = this.validateForm();
    if (errors[field]) {
      this.formErrors[field] = errors[field];
    } else {
      delete this.formErrors[field];
    }
  }

  revalidateTrackField(field: keyof CreateTrack): void {
    if (this.formErrors[field]) {
      this.validateTrackField(field);
    }
  }

  submitTrack(): void {
    if (!this.authService.isLoggedIn()) {
      this.errorMessage = 'You must be signed in to create a track.';
      return;
    }

    this.successMessage = null;
    this.errorMessage = null;
    this.formErrors = this.validateForm();

    if (Object.keys(this.formErrors).length > 0) {
      this.errorMessage = 'Please fix the highlighted fields before creating the track.';
      return;
    }

    this.isSubmitting = true;
    const payload: CreateTrack = {
      title: this.form.title.trim(),
      artistId: Number(this.form.artistId),
      isrc: this.form.isrc.trim().toUpperCase(),
      releaseDate: this.form.releaseDate,
      genre: this.form.genre.trim()
    };

    this.trackService.create(payload).subscribe({
      next: (created) => {
        this.isSubmitting = false;
        if (!this.authService.isLoggedIn()) {
          return;
        }
        this.closeCreateDialog();
        this.successMessage = `Track "${created.title}" was added successfully.`;
        this.load();
      },
      error: (err) => {
        this.isSubmitting = false;
        if (!this.authService.isLoggedIn()) {
          return;
        }
        this.errorMessage = this.extractErrorMessage(err);
      }
    });
  }

  validateForm(): Record<string, string> {
    const errors: Record<string, string> = {};

    if (!this.form.title || this.form.title.trim().length < 2) {
      errors['title'] = 'Title must be at least 2 characters.';
    }
    if (!this.form.artistId || Number(this.form.artistId) <= 0) {
      errors['artistId'] = 'Please select an artist.';
    }
    if (!/^[A-Z0-9]{12}$/.test((this.form.isrc || '').trim().toUpperCase())) {
      errors['isrc'] = 'ISRC must be exactly 12 letters or numbers.';
    }
    if (!this.form.releaseDate || this.form.releaseDate < '2000-01-01') {
      errors['releaseDate'] = 'Release date must be on or after 2000-01-01.';
    }
    if (!this.form.genre || this.form.genre.trim().length < 2) {
      errors['genre'] = 'Genre must be at least 2 characters.';
    }

    return errors;
  }

  extractErrorMessage(err: any): string {
    const body = err?.error;
    if (body?.errors) {
      return Object.values(body.errors).flat().join(' ');
    }
    return body?.detail ?? 'Unable to create the record. Please review the form and try again.';
  }

  statusClass(status: string): string {
    return `status-badge status-${status.toLowerCase()}`;
  }
}
