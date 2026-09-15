import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Dsp, TrackDetail, TrackStatus } from '../models/models';
import { TrackService } from '../services/track.service';
import { DspService } from '../services/dsp.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-track-detail',
  templateUrl: './track-detail.component.html',
  styleUrls: ['./track-detail.component.css'],
  standalone: false
})
export class TrackDetailComponent implements OnInit {
  track: TrackDetail | null = null;
  allDsps: Dsp[] = [];
  selectedDspIds = new Set<number>();

  isLoading = false;
  errorMessage: string | null = null;
  actionMessage: string | null = null;

  readonly statuses: TrackStatus[] = ['Draft', 'Submitted', 'Distributed'];
  newStatus = '';

  private trackId!: number;

  constructor(
    private route: ActivatedRoute,
    private trackService: TrackService,
    private dspService: DspService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.trackId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadTrack();
    this.dspService.getAll().subscribe((dsps) => (this.allDsps = dsps));
  }

  loadTrack(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.trackService.getById(this.trackId).subscribe({
      next: (track) => {
        this.track = track;
        this.newStatus = track.status;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Track not found.';
        this.isLoading = false;
      }
    });
  }

  toggleDsp(dspId: number): void {
    if (this.selectedDspIds.has(dspId)) {
      this.selectedDspIds.delete(dspId);
    } else {
      this.selectedDspIds.add(dspId);
    }
  }

  isDistributedTo(dspId: number): boolean {
    return this.track?.distributions.some((d) => d.dspId === dspId) ?? false;
  }

  submitDistribution(): void {
    if (this.selectedDspIds.size === 0) {
      this.actionMessage = 'Select at least one DSP first.';
      return;
    }

    this.actionMessage = null;
    this.trackService.distribute(this.trackId, Array.from(this.selectedDspIds)).subscribe({
      next: (updated) => {
        this.track = updated;
        this.selectedDspIds.clear();
        this.actionMessage = 'Track submitted to selected DSPs.';
      },
      error: (err) => {
        this.actionMessage = err?.error?.detail ?? 'Failed to submit track to DSPs.';
      }
    });
  }

  submitStatusChange(): void {
    if (!this.newStatus) {
      return;
    }

    this.trackService.updateStatus(this.trackId, this.newStatus).subscribe({
      next: () => {
        this.actionMessage = `Status updated to ${this.newStatus}.`;
        this.loadTrack();
      },
      error: (err) => {
        this.actionMessage = err?.error?.detail ?? 'Failed to update status.';
      }
    });
  }

  statusClass(status: string): string {
    return `status-badge status-${status.toLowerCase()}`;
  }
}
