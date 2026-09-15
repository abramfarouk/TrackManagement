import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Artist, CreateTrack } from '../models/models';
import { ArtistService } from '../services/artist.service';
import { TrackService } from '../services/track.service';

@Component({
  selector: 'app-track-form',
  templateUrl: './track-form.component.html',
  styleUrls: ['./track-form.component.css'],
  standalone: false
})
export class TrackFormComponent implements OnInit {
  artists: Artist[] = [];
  errorMessage: string | null = null;
  isSubmitting = false;

  model: CreateTrack = {
    title: '',
    artistId: 0,
    isrc: '',
    releaseDate: new Date().toISOString().substring(0, 10),
    genre: ''
  };

  constructor(
    private artistService: ArtistService,
    private trackService: TrackService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.artistService.getAll().subscribe((artists) => (this.artists = artists));
  }

  submit(): void {
    this.errorMessage = null;
    this.isSubmitting = true;

    this.trackService.create(this.model).subscribe({
      next: (created) => {
        this.isSubmitting = false;
        this.router.navigate(['/tracks', created.id]);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = this.extractErrorMessage(err);
      }
    });
  }

  private extractErrorMessage(err: any): string {
    const body = err?.error;
    if (body?.errors) {
      return Object.values(body.errors).flat().join(' ');
    }
    return body?.detail ?? 'Failed to create track.';
  }
}
