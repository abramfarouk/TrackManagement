import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CreateTrack, Track, TrackDetail, TrackFilter } from '../models/models';

@Injectable({ providedIn: 'root' })
export class TrackService {
  private readonly baseUrl = `${environment.apiBaseUrl}/tracks`;

  constructor(private http: HttpClient) {}

  getAll(filter: TrackFilter): Observable<Track[]> {
    let params = new HttpParams();
    if (filter.artistId) {
      params = params.set('artistId', filter.artistId);
    }
    if (filter.genre) {
      params = params.set('genre', filter.genre);
    }
    if (filter.status) {
      params = params.set('status', filter.status);
    }
    return this.http.get<Track[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<TrackDetail> {
    return this.http.get<TrackDetail>(`${this.baseUrl}/${id}`);
  }

  create(track: CreateTrack): Observable<Track> {
    return this.http.post<Track>(this.baseUrl, track);
  }

  distribute(trackId: number, dspIds: number[]): Observable<TrackDetail> {
    return this.http.post<TrackDetail>(`${this.baseUrl}/${trackId}/distribute`, { dspIds });
  }

  updateStatus(trackId: number, status: string): Observable<Track> {
    return this.http.patch<Track>(`${this.baseUrl}/${trackId}/status`, { status });
  }
}
