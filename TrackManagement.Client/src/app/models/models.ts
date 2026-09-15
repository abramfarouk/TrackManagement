export type TrackStatus = 'Draft' | 'Submitted' | 'Distributed';

export type DistributionStatus = 'Pending' | 'Live' | 'Rejected';

export interface Artist {
  id: number;
  name: string;
  email: string;
  country: string;
  trackCount: number;
}

export interface CreateArtist {
  name: string;
  email: string;
  country: string;
}

export interface Dsp {
  id: number;
  name: string;
}

export interface TrackDistribution {
  id: number;
  dspId: number;
  dspName: string;
  submittedAt: string;
  status: DistributionStatus;
}

export interface Track {
  id: number;
  title: string;
  artistId: number;
  artistName: string;
  isrc: string;
  releaseDate: string;
  genre: string;
  status: TrackStatus;
}

export interface TrackDetail extends Track {
  distributions: TrackDistribution[];
}

export interface CreateTrack {
  title: string;
  artistId: number;
  isrc: string;
  releaseDate: string;
  genre: string;
}

export interface TrackFilter {
  artistId?: number | null;
  genre?: string | null;
  status?: string | null;
}

export interface ApiError {
  title: string;
  status: number;
  detail: string;
  errors?: Record<string, string[]>;
}
